using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;
using System.Collections;

public class TorrentManager : IEnumerable<Torrent>, IDisposable
{
    Dictionary<string, Torrent> torrents = new Dictionary<string, Torrent>();

    public ConnectInfo MyConnectInfo = new ConnectInfo(new byte[]{ 192, 168, 1, 102 }, 10421);
    public Torrent this[string i]
    {
        get { return torrents[i]; }
        set { torrents[i] = value; }
    }

    public TorrentManager()
    { }

    public TorrentManager(byte[] ipAdress)
    {
        MyConnectInfo = new ConnectInfo(ipAdress, 10421);
    }

    public static TorrentManager FromXml(XmlElement xmlElement)
    {
        TorrentManager tm = new TorrentManager();
        foreach (XmlElement e in xmlElement["torrents"])
        {
            Torrent t = Torrent.CreateFromXml(e);
            tm.Add(t); //later automatic start
        }
        tm.MyConnectInfo = ConnectInfo.ParseXml(xmlElement[ConnectInfo.XmlName]);
        return tm;
    }

    public const string XmlName = "torrentmanager";
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
        return elem;
    }

    public async Task StartListening()
    {
        await startListeningAsync();
    }

    private async Task startListeningAsync()
    {
        Logger.WriteLine("Starting to listen at port : " + MyConnectInfo.Port);
        TcpListener lis = new TcpListener(new IPAddress(MyConnectInfo.IP), MyConnectInfo.Port);
        lis.Start();
        TcpClient c;

        for(;;)
        {
            c = await lis.AcceptTcpClientAsync();
            UpClient mc = new UpClient(c);
            Logger.WriteLine("Client connected. IP: " + ((IPEndPoint)c.Client.RemoteEndPoint).Address.ToString());

            
            int remoteListeningPort = await mc.ReadIntAsync();
            Logger.WriteLine("Clients listening port: " + remoteListeningPort);
            

            string id = await mc.ReadIdAsync();
            Logger.WriteLine("Requests torrent with id: " + id);

            if (torrents.ContainsKey(id) && (torrents[id].Status == Torrent.eStatus.Downloading || torrents[id].Status == Torrent.eStatus.Seeding))
            {
                Logger.WriteLine("Sending OK for torrent with id: " + id);
                await mc.SendByteAsync((byte)Client.eRequestPartResponse.OK);

                if (await mc.ReadByteAsync() == (byte)eConnectType.RequestClients)
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
                ConnectInfo newInfo = new ConnectInfo(((IPEndPoint)c.Client.RemoteEndPoint).Address.GetAddressBytes(), remoteListeningPort);
                if (!torrents[id].ClientsInfo.ContainsValue(newInfo))
                {
                    torrents[id].ClientsInfo.Add(newInfo);
                }
            }
            else
            {
                Logger.WriteLine("Requested torrent is unavailable");

                await mc.SendByteAsync((byte)Client.eRequestPartResponse.NeverAvailable);


                mc.Close();
            }
            
        }
    }

    public async Task ConnectAllDownloadingTorrentsAsync()
    {
        foreach (Torrent torrent in this)
        {
            if (torrent.Status == Torrent.eStatus.Downloading)
            {
                foreach (ConnectInfo info in torrent.ClientsInfo.ToArray())
                {
                    try
                    {
                        await ConnectTorrentAsync(torrent, info);

                    }
                    catch (SocketException)
                    {
                        Logger.WriteLine("Unable to connect to client: " + info.IPToString() + " port: " + info.Port);
                    }
                }
                torrent.Download();
            }
        }
    }

    /*public virtual HashSet<Client> Clients
    {
        get;
        set;
    } = new HashSet<Client>();*/


	public virtual void Add(Torrent t)
	{
        torrents.Add(t.Id, t);
	}

    public virtual void Remove(Torrent t)
    {
        torrents.Remove(t.Id);
        t.Close();
    }

    enum eConnectType
    {
        RequestClients,
        JustConnect
    }

    public async Task ConnectTorrentAsync(Torrent torrent, ConnectInfo connectInfo)
    {
        if (ConnectInfo.Equals(connectInfo, MyConnectInfo))
        {
            return;
        }

        if (!torrent.ClientsInfo.ContainsValue(connectInfo))
            torrent.ClientsInfo.Add(connectInfo);

        DownClient cl = new DownClient();
        
        await cl.ConnectAsync(connectInfo);

        await cl.SendIntAsync(MyConnectInfo.Port);

        await cl.SendIdAsync(torrent.Id);

        if ((Client.eRequestPartResponse)await cl.ReadByteAsync() != Client.eRequestPartResponse.OK)
        {
            Logger.WriteLine("Torrent not available on host: " + connectInfo.IPToString() + " port: " + connectInfo.Port.ToString());
            Logger.WriteLine("Unable to connect to any clients");
            if(!torrent.ClientsInfo.ContainsValue(connectInfo))
                torrent.ClientsInfo.Add(connectInfo);
            return;
        }
        Logger.WriteLine("Requesting clients");
        await cl.SendByteAsync((byte)eConnectType.RequestClients);
        
        int count = await cl.ReadIntAsync();
        Logger.WriteLine("Receiving info of additional clients. Number of clients:" + count);
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
                if ((Client.eRequestPartResponse)await c.ReadByteAsync() != Client.eRequestPartResponse.OK)
                {
                    Logger.WriteLine("Torrent not available on host: " + info.IPToString() + " port: " + info.Port.ToString());
                    continue;
                }

                await c.SendByteAsync((byte)eConnectType.JustConnect);

                Logger.WriteLine("Successfully connected to client: " + c.ConnectInfo.IPToString() + " Port:" + port.ToString());

                torrent.AddClient(c);
                torrent.ClientsInfo.Add(info);  
            }
            catch (SocketException ex)
            {
                Logger.WriteLine(ex.Message);
            }
        }

        torrent.AddClient(cl);
    }

    public IEnumerator<Torrent> GetEnumerator()
    {
        return torrents.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
        foreach (Torrent t in this)
            t.Dispose();
    }
}

public class TorrentNotAvailableException : Exception
{

    public TorrentNotAvailableException(string v) : base(v)
    {
        
    }
}