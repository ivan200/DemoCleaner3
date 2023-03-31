using System;
using System.Xml;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DemoCleaner3 {
    public static class XmlUtils {

        // normalize keys for xml attributes
        private static String NormalizeAttributeName(String name) {
            String output = "";
            if (name.Length == 0 || (name[0] >= '0' && name[0] <= '9')) {
                output += "_";
            }
            output += Regex.Replace(name, "([\\W-])", "");
            return output;
        }

        // Remove characters like "&#xa;" or "&#xE;" from output
        static String FilterUnicode(String name) {
            return Regex.Replace(name, "(&#x[0-9a-fA-F]{1,4};)", "");
        }

        public static XmlNode ExportXml(XmlDocument doc, Dictionary<string, Dictionary<string, string>> map) {
            var node = doc.CreateNode("element", "demoFile", "");
            foreach (var item in map) {
                var el = doc.CreateNode("element", NormalizeAttributeName(item.Key), "");
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
            return FilterUnicode(ExportXml(doc, friendlyInfo).OuterXml);
        }
    }
}
