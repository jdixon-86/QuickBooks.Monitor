using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace FileStateEnforcer
{
    public static class Settings
    {
        public static IEnumerable<LineRule> ImportSettings(XDocument doc)
        {
            var defaultElement = doc.Element("watcher").Element("default");
            var defaultData = new LineRule(
                directory: defaultElement.Element("path").Value,
                filename: defaultElement.Element("filename").Value,
                search: defaultElement.Element("line").Value,
                rule: defaultElement.Element("replace").Value);

            return doc.Element("watcher").Elements("data").Select(e =>
                new LineRule(
                    directory: defaultElement.Element("path").Value,
                    filename: defaultElement.Element("filename").Value,
                    search: defaultElement.Element("line").Value,
                    rule: defaultElement.Element("replace").Value));
        }
    }

    public static class XContainerMixins
    {
        public static string ElementValueOrDefault(this XContainer This, string key, string defaultVal)
        {
            var e = This.Element(key);
            return e == null ? defaultVal : e.Value;
        }
    }
}
