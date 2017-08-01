using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;



public static class XmlExtension
{
    public static XmlElement CreateElementWithValue(this XmlDocument doc, string xmlName, string value)
    {
        XmlElement elem = doc.CreateElement(xmlName);
        elem.InnerText = value;
        return elem;
    }

    public static void AppendElementWithValue(this XmlElement el, string xmlName, string value)
    {
        XmlElement elem = el.OwnerDocument.CreateElement(xmlName);
        elem.InnerText = value;
        el.AppendChild(elem);
    }
}

