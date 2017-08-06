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

    public static TorrentManager FromXml(XmlElement xmlElement)
    {
        TorrentManager tm = new TorrentManager();
        foreach (XmlElement e in xmlElement["torrents"])
        {
            Torrent t = Torrent.CreateFromXml(e);
            tm.Add(t); //later automatic start
        }
        if (xmlElement.GetElementsByTagName(ConnectInfo.XmlName).Count == 1)
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

        elem.AppendChild(MyConnectInfo.ShareHeaderToXml(doc));
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
            Client mc = new Client(c);
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
                    Logger.WriteLine("Sending number of clients: " + torrents[id].Clients.Count);
                    await mc.SendIntAsync(torrents[id].Clients.Count);

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
                Clients.Add(mc);
                torrents[id].ClientsInfo.Add(new ConnectInfo(((IPEndPoint)c.Client.RemoteEndPoint).Address.GetAddressBytes(), remoteListeningPort));
            }
            else
            {
                Logger.WriteLine("Requested torrent is unavailable");

                await mc.SendByteAsync((byte)Client.eRequestPartResponse.NeverAvailable);


                mc.Close();
            }
            
        }
    }

    public virtual HashSet<Client> Clients
    {
        get;
        set;
    } = new HashSet<Client>();


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
        Client cl = new Client();
        
        await cl.ConnectAsync(connectInfo);

        await cl.SendIntAsync(MyConnectInfo.Port);

        await cl.SendIdAsync(torrent.Id);

        if ((Client.eRequestPartResponse)await cl.ReadByteAsync() != Client.eRequestPartResponse.OK)
        {
            Logger.WriteLine("Torrent not available on host: " + connectInfo.IPToString() + " port: " + connectInfo.Port.ToString());
            torrent.Status = Torrent.eStatus.Error;
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

            Logger.WriteLine("Trying to connect to client: " + info.IPToString() + " Port:" + port.ToString());

            try
            {
                Client c = new Client();
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
                Clients.Add(c);
                torrent.Clients.Add(c);
                torrent.ClientsInfo.Add(info);
            }
            catch (SocketException)
            { }
        }


        Add(torrent);

        Clients.Add(cl);
        torrent.Clients.Add(cl);
        torrent.ClientsInfo.Add(connectInfo);
        await torrent.Download();
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