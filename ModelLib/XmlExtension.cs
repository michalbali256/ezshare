using System.Xml;

namespace EzShare
{
    namespace ModelLib
    {
        /// <summary>
        /// Provides extension methods for XmlDocument and XmlElement
        /// </summary>
        public static class XmlExtension
        {
            /// <summary>
            /// Creates new element using XmlDocument.CreateElement with specified name and value.
            /// </summary>
            /// <param name="doc">The context XmlDocument</param>
            /// <param name="xmlName">Name of new xml element</param>
            /// <param name="value">Value that will be written into innter text of new element</param>
            /// <returns>Newly created XmlElement.</returns>
            public static XmlElement CreateElementWithValue(this XmlDocument doc, string xmlName, string value)
            {
                XmlElement elem = doc.CreateElement(xmlName);
                elem.InnerText = value;
                return elem;
            }
            /// <summary>
            /// Appends newly created element with specified name and value
            /// </summary>
            /// <param name="parentXmlElement">The XmlElement to which new element will be appended</param>
            /// <param name="xmlName">Name of new xml element</param>
            /// <param name="value">Value that will be written into innter text of new element</param>
            public static void AppendElementWithValue(this XmlElement parentXmlElement, string xmlName, string value)
            {
                XmlElement newElement = parentXmlElement.OwnerDocument.CreateElement(xmlName);
                newElement.InnerText = value;
                parentXmlElement.AppendChild(newElement);
            }
        }
    }
}

