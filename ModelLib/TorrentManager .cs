﻿using System;
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

            string id = await mc.ReceiveIdAsync();
            Logger.WriteLine("Requests torrent with id: " + id);

            if (torrents.ContainsKey(id) && (torrents[id].Status == Torrent.eStatus.Downloading || torrents[id].Status == Torrent.eStatus.Seeding))
            {
                await mc.SendByte((byte)Client.eRequestPartResponse.OK);

                /*await mc.SendInt(torrents[id].Clients.Count);

                foreach (Client cl in torrents[id].Clients)
                {
                    await cl.SendBytesAsync(cl.ConnectInfo.IP);
                    await cl.SendInt(cl.ConnectInfo.Port);
                }*/


                torrents[id].AddClient(mc);
                Clients.Add(mc);
            }
            else
            {
                Logger.WriteLine("Requested torrent is unavailable");

                await mc.SendByte((byte)Client.eRequestPartResponse.NeverAvailable);


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

    

    public async Task ConnectTorrentAsync(Torrent torrent, ConnectInfo connectInfo)
    {
        Client cl = new Client();
        
        await cl.ConnectAsync(connectInfo);
        
        await cl.SendIdAsync(torrent.Id);

        if ((Client.eRequestPartResponse)await cl.ReadByteAsync() != Client.eRequestPartResponse.OK)
        {
            Logger.WriteLine("Torrent not available on host.");
            torrent.Status = Torrent.eStatus.Error;
            return;
        }

        /*int count = await cl.ReadInt();

        for(int i = 0; i < count; ++i)
        {
            await cl.SendBytesAsync(cl.ConnectInfo.IP);
            await cl.SendInt(cl.ConnectInfo.Port);
        }*/


        Add(torrent);

        Clients.Add(cl);
        torrent.Clients.Add(cl);
        
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