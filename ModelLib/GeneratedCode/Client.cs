using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
public class Client
{
	public virtual object IP
	{
		get;
		set;
	}

	public virtual object port
	{
		get;
		set;
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

