﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using System.Collections;
using System.IO;

namespace EzShare
{
    namespace ModelLib
    {

        /// <summary>
        /// Acts as collection of torrents, provides Listener and methods for creating new connections
        /// </summary>
        public class TorrentManager : IEnumerable<Torrent>, IDisposable
        {
            public const string XmlName = "torrentmanager";
            public bool ForceIp = false;
            /// <summary>
            /// Collection of torrents managed by this manager
            /// </summary>
            Dictionary<string, Torrent> torrents = new Dictionary<string, Torrent>();

            /// <summary>
            /// Connection info of this manager - it will listen on this IP and port
            /// </summary>
            public ConnectInfo MyConnectInfo = new ConnectInfo(new byte[] { 192, 168, 1, 102 }, 10421);
            public Torrent this[string i]
            {
                get { return torrents[i]; }
                set { torrents[i] = value; }
            }



            /// <summary>
            /// Constructs new empty manager. Network interface used for listening determined automatically.
            /// </summary>
            public TorrentManager()
            {
                GetLocalIpAddress();
            }
            /// <summary>
            /// Constructs new empty manager.
            /// </summary>
            /// <param name="ipAdress">IP address of this computer - specifies network interface to listen on.</param>
            public TorrentManager(byte[] ipAdress)
            {
                MyConnectInfo = new ConnectInfo(ipAdress, 10421);
            }



            enum EConnectType
            {
                RequestClients,
                JustConnect
            }



            /// <summary>
            /// Sends a packet to an adress to determine preferred network interface of device
            /// </summary>
            /// <returns>Returns local IP of preferred interface of this device</returns>
            private static byte[] GetLocalIpAddress()
            {
                using (Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, 0))
                {
                    socket.Connect("8.8.8.8", 65530);
                    IPEndPoint endPoint = socket.LocalEndPoint as IPEndPoint;
                    return endPoint.Address.GetAddressBytes();
                }
            }

            /// <summary>
            /// Constructs manager from XmlElement
            /// </summary>
            /// <param name="xmlElement"></param>
            /// <returns></returns>
            public static TorrentManager FromXml(XmlElement xmlElement)
            {
                TorrentManager torrentManager = new TorrentManager();
                foreach (XmlElement e in xmlElement["torrents"])
                {
                    Torrent t = Torrent.CreateFromXml(e);
                    torrentManager.Add(t); //later automatic start
                }
                
                torrentManager.ForceIp = bool.Parse(xmlElement["forceip"].InnerText);
                ConnectInfo loadedConnectInfo = ConnectInfo.ParseXml(xmlElement[ConnectInfo.XmlName]);
                if (torrentManager.ForceIp)
                    torrentManager.MyConnectInfo = loadedConnectInfo;
                else
                    torrentManager.MyConnectInfo = new ConnectInfo(GetLocalIpAddress(), loadedConnectInfo.Port);

                return torrentManager;
            }
            
            /// <summary>
            /// Serializes this manager into Xml.
            /// </summary>
            /// <param name="doc">Document context</param>
            /// <returns>Returns new XmlElement that represents this</returns>
            public XmlElement SaveToXml(XmlDocument doc)
            {
                XmlElement elem = doc.CreateElement(XmlName);
                XmlElement torrentsElem = doc.CreateElement("torrents");
                foreach (Torrent t in torrents.Values)
                {
                    torrentsElem.AppendChild(t.SaveToXml(doc));
                }
                elem.AppendChild(torrentsElem);

                elem.AppendChild(MyConnectInfo.SaveToXml(doc));
                elem.AppendElementWithValue("forceip", ForceIp.ToString());
                return elem;
            }

            /// <summary>
            /// Starts listening for new connections on MyConnectionInfo. Manages newly created clients.
            /// </summary>
            /// <returns></returns>
            public async Task StartListeningAsync()
            {
                Logger.WriteLine("Starting to listen at port : " + MyConnectInfo.Port);
                TcpListener lis = new TcpListener(new IPAddress(MyConnectInfo.IP), MyConnectInfo.Port);
                lis.Start();
                TcpClient client;

                for (;;)
                {
                    client = await lis.AcceptTcpClientAsync();
                    UpClient mc = new UpClient(client);
                    Logger.WriteLine("Client connected. IP: " + ((IPEndPoint)client.Client.RemoteEndPoint).Address);

                    int remoteListeningPort = await mc.ReadIntAsync();
                    Logger.WriteLine("Clients listening port: " + remoteListeningPort);


                    string id = await mc.ReadIdAsync();
                    Logger.WriteLine("Requests torrent with id: " + id);

                    //if there is torrent with this id that can be seeded, create new UpClient that will share this torrent.
                    if (torrents.ContainsKey(id) && (torrents[id].Status == Torrent.EStatus.Downloading || torrents[id].Status == Torrent.EStatus.Seeding))
                    {
                        Logger.WriteLine("Sending OK for torrent with id: " + id);
                        await mc.SendByteAsync((byte)Client.ERequestPartResponse.OK);
                        //sends information about all clients that could seed this torrent
                        if (await mc.ReadByteAsync() == (byte)EConnectType.RequestClients)
                        {
                            Logger.WriteLine("Sending number of clients: " + torrents[id].ClientsInfo.Count);
                            await mc.SendIntAsync(torrents[id].ClientsInfo.Count);

                            foreach (ConnectInfo cl in torrents[id].ClientsInfo)
                            {
                                Logger.WriteLine("Sending client IP: " + cl.IPToString());
                                await mc.SendBytesAsync(cl.IP);
                                Logger.WriteLine("Sending client port: " + cl.Port.ToString());
                                await mc.SendIntAsync(cl.Port);
                            }
                        }
                        else
                        {
                            Logger.WriteLine("Clients not requested");
                        }
                        torrents[id].AddClient(mc);
                        mc.ListenAsync(torrents[id]);
                        ConnectInfo newInfo = new ConnectInfo(((IPEndPoint)client.Client.RemoteEndPoint).Address.GetAddressBytes(), remoteListeningPort);
                        if (!torrents[id].ClientsInfo.ContainsValue(newInfo))
                        {
                            torrents[id].ClientsInfo.Add(newInfo);
                        }
                    }
                    else
                    {
                        Logger.WriteLine("Requested torrent is unavailable");

                        await mc.SendByteAsync((byte)Client.ERequestPartResponse.NeverAvailable);

                        mc.Close();
                    }
                }
            }

            /// <summary>
            /// Tries to connect all clients of all downloading torrrents and resumes download.
            /// </summary>
            /// <returns></returns>
            public async Task ConnectAllDownloadingTorrentsAsync()
            {
                //connecting to all torrents may take long time and user might change torrents - need to connect to existing torrents anyway
                foreach (Torrent torrent in this.ToList())
                {
                    if (torrent.Status == Torrent.EStatus.Downloading)
                        ConnectDownloadingTorrentAsync(torrent);

                }
            }

            public async Task ConnectDownloadingTorrentAsync(Torrent torrent)
            {
                if (torrent.Status == Torrent.EStatus.Downloading)
                {
                    List<Task> connectTasks = new List<Task>();
                    Dictionary<Task, ConnectInfo> dict = new Dictionary<Task, ConnectInfo>();

                    foreach (ConnectInfo info in torrent.ClientsInfo.ToList())
                    {
                        Task task = ConnectTorrentAsync(torrent, info);
                        connectTasks.Add(task);
                        dict.Add(task, info);
                    }

                    Task<Task> whenAny = Task.WhenAny(connectTasks);
                    whenAny.Start();
                    while (torrent.Clients.Count == 0)
                    {
                        Task frst = await whenAny;
                        if (frst.IsFaulted)
                        {
                            Logger.WriteLine("Unable to connect to client. IP: " + dict[frst].IPToString() + " Port: " + dict[frst].Port.ToString());
                            foreach (Exception exception in frst.Exception.InnerExceptions)
                                Logger.WriteLine(exception.Message);
                        }
                    }

                    await torrent.DownloadAsync();
                }
            }

            /// <summary>
            /// Checks if port is used with the IP in this computer
            /// </summary>
            /// <param name="checkInfo">The IP and port to check (IP specifies network interface)</param>
            /// <returns></returns>
            public static bool IsPortUsed(ConnectInfo checkInfo)
            {
                var listeners = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();

                foreach (IPEndPoint listener in listeners)
                {
                    if (listener.Port == checkInfo.Port && listener.Address.GetAddressBytes().SequenceEqual(checkInfo.IP))
                    {
                        return true;
                    }
                }

                return false;
            }

            /// <summary>
            /// Saves share file of specified torrent to specified location.
            /// </summary>
            /// <param name="torrent">The torrent of which to save .share file</param>
            /// <param name="shareFileName">File name of new .share file</param>
            /// <exception cref="IOException"></exception>
            public void SaveShareFile(Torrent torrent, string shareFileName)
            {

                XmlDocument doc = new XmlDocument();
                XmlElement documentElement = doc.CreateElement("share");
                documentElement.AppendChild(MyConnectInfo.SaveToXml(doc));
                documentElement.AppendChild(torrent.SaveToXmlShare(doc));

                doc.AppendChild(documentElement);
                try
                {
                    doc.Save(shareFileName);
                    Logger.WriteLine("Successfuly written " + shareFileName);
                }
                catch (IOException exception)
                {
                    Logger.WriteLine("Could not save to specified location: " + shareFileName + " Reason: " + exception.Message);
                    throw;
                }

            }

            /// <summary>
            /// Adds torrent, must be unique
            /// </summary>
            /// <param name="torrent">The torrent to add</param>
            public virtual void Add(Torrent torrent)
            {
                torrents.Add(torrent.Id, torrent);
            }

            /// <summary>
            /// Removes specified torrent and closes it
            /// </summary>
            /// <param name="torrent">The torrent to remove</param>
            public virtual void Remove(Torrent torrent)
            {
                torrents.Remove(torrent.Id);
                torrent.Close();
            }

            /// <summary>
            /// Creates new DownClient and connects it to remote host.
            /// </summary>
            /// <param name="torrent">The torrent to download with new client</param>
            /// <param name="connectInfo">IP and port of remote endpoint from which download</param>
            /// <returns></returns>
            public async Task ConnectTorrentAsync(Torrent torrent, ConnectInfo connectInfo)
            {
                //makes no sense to connect to myself
                if (ConnectInfo.Equals(connectInfo, MyConnectInfo))
                    return;
                

                if (!torrent.ClientsInfo.ContainsValue(connectInfo))
                    torrent.ClientsInfo.Add(connectInfo);

                
                DownClient cl = new DownClient();

                Logger.WriteLine("Trying to connect to client: " + connectInfo.IPToString() + " port: " + connectInfo.Port.ToString());

                await cl.ConnectAsync(connectInfo);
                
                await cl.SendIntAsync(MyConnectInfo.Port); //sends listening port of this instance

                await cl.SendIdAsync(torrent.Id);//torrent to download

                if ((Client.ERequestPartResponse)await cl.ReadByteAsync() != Client.ERequestPartResponse.OK)
                {
                    Logger.WriteLine("Torrent not available on host: " + connectInfo.IPToString() + " port: " + connectInfo.Port.ToString());
                    Logger.WriteLine("Unable to connect to any clients");
                    if (!torrent.ClientsInfo.ContainsValue(connectInfo))
                        torrent.ClientsInfo.Add(connectInfo);
                    return;
                }
                Logger.WriteLine("Requesting clients");
                await cl.SendByteAsync((byte)EConnectType.RequestClients);

                int count = await cl.ReadIntAsync();
                Logger.WriteLine("Receiving info of additional clients. Number of clients: " + count);
                for (int i = 0; i < count; ++i)
                {
                    Logger.WriteLine("Receiving IP");
                    byte[] ip = await cl.ReadBytesAsync(4);
                    Logger.WriteLine("Receiving Port");
                    int port = await cl.ReadIntAsync();
                    ConnectInfo info = new ConnectInfo(ip, port);

                    if (torrent.ClientsInfo.ContainsValue(info))
                    {
                        Logger.WriteLine("Client already in clients list");
                        continue;
                    }
                    if (ConnectInfo.Equals(info, MyConnectInfo))
                    {
                        Logger.WriteLine("Recieved info about this client.");
                        continue;
                    }
                    Logger.WriteLine("Trying to connect to client: " + info.IPToString() + " Port:" + port.ToString());

                    try
                    {
                        DownClient c = new DownClient();
                        await c.ConnectAsync(info);

                        await c.SendIntAsync(info.Port);

                        await c.SendIdAsync(torrent.Id);
                        if ((Client.ERequestPartResponse)await c.ReadByteAsync() != Client.ERequestPartResponse.OK)
                        {
                            Logger.WriteLine("Torrent not available on host: " + info.IPToString() + " port: " + info.Port.ToString());
                            continue;
                        }

                        await c.SendByteAsync((byte)EConnectType.JustConnect);

                        Logger.WriteLine("Successfully connected to client: " + c.ConnectInfo.IPToString() + " Port:" + port.ToString());

                        torrent.AddClient(c);
                        torrent.ClientsInfo.Add(info);
                    }
                    catch (SocketException ex)
                    {//if there is problem connecting to one of additional hosts, it should be no real problem.
                        Logger.WriteLine(ex.Message);
                    }
                }

                torrent.AddClient(cl);
            }

            public IEnumerator<Torrent> GetEnumerator()
            {
                return torrents.Values.GetEnumerator();
            }

            public void Dispose()
            {
                foreach (Torrent t in this)
                    t.Dispose();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}