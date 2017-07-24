using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

public class TorrentManager
{
    List<Torrent> torrents = new List<Torrent>();
    byte[] myIP = { 192, 168, 0, 100 };
    int port = 10421;

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

    static string shareHeaderXmlName = "host";

    public XmlElement ShareHeaderToXml(XmlDocument doc)
    {
        XmlElement el = doc.CreateElement(shareHeaderXmlName);
        XmlElement ip = doc.CreateElement("ip");
        ip.InnerText = $"{myIP[0]}.{myIP[1]}.{myIP[2]}.{myIP[3]}";
        XmlElement port = doc.CreateElement("port");
        port.InnerText = this.port.ToString();

        el.AppendChild(ip);
        el.AppendChild(port);

        return el;
    }
}

