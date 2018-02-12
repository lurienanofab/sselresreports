using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;
using System.IO;

namespace sselResReports.AppCode.Rss
{
    public class RssFeed
    {
        public string Version { get; set; }
        public Channel Channel { get; set; }

        public XmlDocument ToXml()
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml("<rss/>");

            AddAttributes(xdoc.DocumentElement, new Dictionary<string, string> { { "version", Version } });

            XmlNode channel = AddNode(xdoc.DocumentElement, "channel");

            AddNode(channel, "title", Channel.Title);
            AddNode(channel, "link", Channel.Link);
            AddNode(channel, "description", Channel.Description);
            AddNode(channel, "pubDate", ConvertDate(Channel.PubDate));
            AddNode(channel, "webMaster", Channel.WebMaster);
            AddNode(channel, "lastBuildDate", ConvertDate(Channel.LastBuildDate));

            AddItems(channel);

            XmlDeclaration dec = xdoc.CreateXmlDeclaration("1.0", null, null);
            xdoc.InsertBefore(dec, xdoc.DocumentElement);

            return xdoc;
        }

        private XmlNode AddNode(XmlNode parent, string name, string text = null, IDictionary<string, string> attributes = null)
        {
            XmlNode node = parent.OwnerDocument.CreateElement(name);
            if (!string.IsNullOrEmpty(text))
                node.InnerText = text;
            AddAttributes(node, attributes);
            parent.AppendChild(node);
            return node;
        }

        private void AddAttributes(XmlNode node, IDictionary<string, string> attributes)
        {
            if (attributes == null)
                return;

            foreach (KeyValuePair<string, string> kvp in attributes)
            {
                XmlAttribute attr = node.OwnerDocument.CreateAttribute(kvp.Key);
                attr.Value = kvp.Value;
                node.Attributes.Append(attr);
            }
        }

        private string GetXmlString(XmlDocument xdoc)
        {
            MemoryStream ms = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.IndentChars = "    ";
            settings.NewLineChars = "\r\n";
            settings.NewLineHandling = NewLineHandling.Replace;
            settings.Encoding = Encoding.UTF8;

            using (XmlWriter writer = XmlWriter.Create(ms, settings))
                xdoc.Save(writer);

            ms.Position = 0;

            using (StreamReader reader = new StreamReader(ms))
                return reader.ReadToEnd();
        }

        private string ConvertDate(DateTime d)
        {
            DateTime utc = d.ToUniversalTime();
            return utc.ToString("ddd, dd MMM yyyy HH':'mm':'ss 'GMT'");
        }

        private void AddItems(XmlNode channel)
        {
            if (Channel.Items != null && Channel.Items.Count > 0)
            {
                foreach (Item i in Channel.Items)
                {
                    XmlNode item = AddNode(channel, "item");
                    AddNode(item, "title", i.Title);
                    AddNode(item, "link", i.Link);
                    AddNode(item, "description", i.Description);
                    AddNode(item, "pubDate", ConvertDate(i.PubDate));
                    AddNode(item, "guid", i.Guid);
                }
            }
        }

        public override string ToString()
        {
            XmlDocument xdoc = ToXml();
            return GetXmlString(xdoc);
        }
    }

    public class Channel
    {
        public string Title { get; set; }//= string.Format("LNF Users in {0}", roomName);
        public DateTime PubDate { get; set; }//= DateTime.Now.ToLongDateString();
        public DateTime LastBuildDate { get; set; }//= DateTime.Now.ToLongDateString();
        public string WebMaster { get; set; }//= "lnf-it@umich.com";
        public string Description { get; set; }//= "Displays a list of all users currently in one of the LNF lab areas";
        public string Link { get; set; }//= "~/AccInLab.aspx";
        public List<Item> Items { get; set; }
    }

    public class Item
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Link { get; set; }
        public string Guid { get; set; }
        public DateTime PubDate { get; set; }
    }
}
