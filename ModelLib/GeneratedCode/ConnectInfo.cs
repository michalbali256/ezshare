using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

public struct ConnectInfo
{
    public ConnectInfo(byte[] ip, int port)
    {
        IP = ip;
        Port = port;
    }

    public byte[] IP;
    public int Port;

    public static string XmlName = "connectinfo";

    public XmlElement ShareHeaderToXml(XmlDocument doc)
    {
        XmlElement el = doc.CreateElement(XmlName);
        XmlElement ip = doc.CreateElement("ip");
        ip.InnerText = IPToString();
        XmlElement port = doc.CreateElement("port");
        port.InnerText = this.Port.ToString();

        el.AppendChild(ip);
        el.AppendChild(port);

        return el;
    }

    public static ConnectInfo ParseXml(XmlElement elem)
    {
        var split = elem["ip"].InnerText.Split('.');
        return new ConnectInfo(
            new byte[]{
                byte.Parse(split[0]),
                byte.Parse(split[1]),
                byte.Parse(split[2]),
                byte.Parse(split[3])},
            int.Parse(elem["port"].InnerText));
    }

    public string IPToString()
    {
        return $"{IP[0]}.{IP[1]}.{IP[2]}.{IP[3]}";
    }
}

