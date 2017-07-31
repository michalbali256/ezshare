using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Threading.Tasks;

using System.Net.Sockets;
using System.Net;

public class TorrentManager
{
    Dictionary<string, Torrent> torrents = new Dictionary<string, Torrent>();

    public ConnectInfo MyConnectInfo = new ConnectInfo(new byte[]{ 192, 168, 1, 102 }, 10421);
    public Torrent this[string i]
    {
        get { return torrents[i]; }
        set { torrents[i] = value; }
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
}

