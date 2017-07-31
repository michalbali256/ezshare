using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
public class Client
{
    private TcpClient client;
    private NetworkStream stream;

    public Client()
    {
        client = new TcpClient();
        stream = client.GetStream();
        
    }

    public Client(TcpClient c)
    {
        this.client = c;
        stream = c.GetStream();
        
        /*byte[] buffer = new byte[1000];
        str.Read(buffer, 0, 100);
        StringBuilder b = new StringBuilder();
        for (int i = 0; i < 100; ++i)
            b.Append((char)buffer[i]);*/
    }

    public async void ConnectAsync(ConnectInfo info)
    {
        await client.ConnectAsync(new IPAddress(info.IP), info.Port);
    }

    /// <summary>
    /// Sends id.
    /// </summary>
    /// <param name="id">The id that should be 32 chars long</param>
    public async void SendIdAsync(string id)
    {
        byte[] buffer = new byte[32];
        for (int i = 0; i < 32; ++i)
            buffer[i] = (byte)id[i];
        await stream.WriteAsync(buffer, 0, 32);
    }

    public async Task<string> ReceiveIdAsync()
    {
        byte[] buffer = new byte[32];
        await stream.ReadAsync(buffer, 0, 32);

        StringBuilder b = new StringBuilder();
        for (int i = 0; i < 32; ++i)
            b.Append((char)buffer[i]);
        return b.ToString();
    }


    public virtual void SendFileInfo()
	{
		throw new System.NotImplementedException();

        TcpClient cl = new TcpClient("192.168.1.100", 9696);
        var ns = cl.GetStream();
        string s = "toto je velmi dlha sprava ktoru by som si chcel poslad to svojho pocitaca mala by mat 100 znakov alen neviem ze ake dlhe toto je bal babnasd";

        byte[] buffer = new byte[1000];
        for (int i = 0; i < 100; ++i)
            buffer[i] = (byte)s[i];

        ns.Write(buffer, 0, 100);
        ns.Flush();
        ns.Close();
    }

	public virtual void SendTorrentInfo()
	{
		throw new System.NotImplementedException();
	}

	public virtual void SendPart()
	{
		throw new System.NotImplementedException();
	}

	public virtual void RequestTorrentInfo()
	{
		throw new System.NotImplementedException();
	}

	public virtual void RequestFileInfo()
	{
		throw new System.NotImplementedException();
	}

	public virtual void RequestPart()
	{
		throw new System.NotImplementedException();
	}

}

