using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace QuickBooks_Monitor
{
    public class QBMonitorSettings
    {
        public QBMonitorSettings(XDocument doc)
        {
            var defaultElement = doc.Element("watcher").Element("default");
            var defaultData = new QuickBooksPath
            {
                Path = defaultElement.Element("path").Value,
                Filename = defaultElement.Element("filename").Value,
                LookFor = defaultElement.Element("line").Value,
                ReplaceWith = defaultElement.Element("replace").Value,
            };

            _paths = doc.Element("watcher").Elements("data").Select(e =>
                {
                    return new QuickBooksPath
                        {
                            Path = e.ElementValueOrDefault("path", defaultData.Path),
                            Filename = e.ElementValueOrDefault("filename", defaultData.Filename),
                            LookFor = e.ElementValueOrDefault("line", defaultData.LookFor),
                            ReplaceWith = e.ElementValueOrDefault("replace", defaultData.ReplaceWith),
                        };
                })
                .ToList();
        }

        private readonly IEnumerable<QuickBooksPath> _paths;
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
