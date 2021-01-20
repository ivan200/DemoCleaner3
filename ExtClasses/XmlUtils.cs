using System;
using System.Xml;
using System.Collections.Generic;

namespace DemoCleaner3 {
    public static class XmlUtils {
        private static String NormalizeAttributeName(String name) {
            String output = "";
            if (name.Length == 0 || (name[0] >= '0' && name[0] <= '9') )
            {
                output += "_";
            }
            foreach (var c in name)
            {
                if ((c >= '0' && c <= '9') || (c >= 'a' && c <= 'z') ||(c >= 'A' && c <= 'Z') || (c == '-'))
                {
                    output += c;
                }
                else
                {
                    output += "_";
                }
            }
            return output;
        }

        public static XmlNode ExportXml(XmlDocument doc, Dictionary<string, Dictionary<string, string>> map) {
            var node = doc.CreateNode("element", "demoFile", "");
            foreach (var item in map) {
                var el = doc.CreateNode("element", item.Key, "");
                foreach (var subItem in item.Value) {
                    var attr = doc.CreateAttribute(NormalizeAttributeName(subItem.Key));
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
