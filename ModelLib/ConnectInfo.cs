using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace EzShare
{
    namespace ModelLib
    {
        /// <summary>
        /// Provides information about endpoint.
        /// </summary>
        public struct ConnectInfo
        {
            /// <summary>
            /// Creates new Connectinfo with specified IP and port.
            /// </summary>
            /// <param name="ip"></param>
            /// <param name="port"></param>
            public ConnectInfo(byte[] ip, int port)
            {
                IP = ip;
                Port = port;
            }

            public byte[] IP;
            public int Port;

            public static string XmlName = "connectinfo";
            /// <summary>
            /// Serializes into xml
            /// </summary>
            /// <param name="doc"></param>
            /// <returns></returns>
            public XmlElement SaveToXml(XmlDocument doc)
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
            /// <summary>
            /// Deserializes from xml
            /// </summary>
            /// <param name="elem"></param>
            /// <returns></returns>
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

            /// <summary>
            /// Returns IP in common notation.
            /// </summary>
            /// <returns></returns>
            public string IPToString()
            {
                return $"{IP[0]}.{IP[1]}.{IP[2]}.{IP[3]}";
            }

            public static bool Equals(ConnectInfo x, ConnectInfo y)
            {
                return x.Port == y.Port && x.IP.SequenceEqual(y.IP);
            }


        }

        
        public static class Extend
        {
            /// <summary>
            /// Checks whether hashset contains the connectinfo
            /// </summary>
            /// <param name="set"></param>
            /// <param name="info"></param>
            /// <returns></returns>
            public static bool ContainsValue(this HashSet<ConnectInfo> set, ConnectInfo info)
            {
                foreach (ConnectInfo i in set)
                    if (ConnectInfo.Equals(i, info))
                        return true;
                return false;
            }
        }
    }
}