using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;
using System.Collections;

public class TorrentManager : IEnumerable<Torrent>
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
        return elem;
    }

    public async void StartListening()
    {
        await startListeningAsync();
    }

    private async Task startListeningAsync()
    {

        TcpListener lis = new TcpListener(new IPAddress(MyConnectInfo.IP), MyConnectInfo.Port);
        lis.Start();
        TcpClient c;

        while (true)
        {
            c = await lis.AcceptTcpClientAsync();

            Client mc = new Client(c);

            string id = await mc.ReceiveIdAsync();

            torrents[id].AddClient(mc);
            Clients.Add(mc);

        }



    }

    public virtual HashSet<Client> Clients
	{
		get;
		set;
	}


	public virtual void Add(Torrent t)
	{
        torrents.Add(t.Id, t);
	}

    public virtual void Remove(Torrent t)
    {
        torrents.Remove(t.Id);
    }

    

    public async Task ConnectTorrentAsync(Torrent torrent, ConnectInfo connectInfo)
    {
        Client cl = new Client();
        
        await cl.ConnectAsync(connectInfo);
        
        await cl.SendIdAsync(torrent.Id);

        //request for additional seeds and connect them//
        //use try catch - not connecting here is not an error

        Add(torrent);
        
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
}

