using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class TorrentManager
{
    List<Torrent> torrents = new List<Torrent>();

    public Torrent this[int i]
    {
        get { return torrents[i]; }
        set { torrents[i] = value; }
    }

	public virtual object Listener
	{
		get;
		set;
	}

	public virtual object Clients
	{
		get;
		set;
	}


	public virtual void Add(Torrent t)
	{
        torrents.Add(t);
	}

    public virtual void Remove(Torrent t)
    {
        torrents.Remove(t);
    }

}

