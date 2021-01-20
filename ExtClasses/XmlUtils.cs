using System.Xml;
using System.Collections.Generic;

namespace DemoCleaner3 {
    public static class XmlUtils {
        public static XmlNode ExportXml(XmlDocument doc, Dictionary<string, Dictionary<string, string>> map) {
            var node = doc.CreateNode("element", "demoFile", "");
            foreach (var item in map) {
                var el = doc.CreateNode("element", item.Key, "");
                foreach (var subItem in item.Value) {
                    var attr = doc.CreateAttribute(subItem.Key.Replace(' ', '-'));
                    attr.Value = subItem.Value;
                    el.Attributes.Append(attr);
                }
                node.AppendChild(el);
            }
            return node;
        }

        public static string FriendlyInfoToXmlString(Dictionary<string, Dictionary<string, string>> friendlyInfo) {
            XmlDocument doc = new XmlDocument();
            return ExportXml(doc, friendlyInfo).OuterXml;
        }

    }

}
