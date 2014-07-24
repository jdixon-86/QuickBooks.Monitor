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
            _paths = doc.Element("watcher").Elements("data").Select(e =>
                {
                    return new QuickBooksPath
                        {
                            Path = e.Element("path").Value,
                            Filename = e.Element("filename").Value,
                            LookFor = e.Element("line").Value,
                            ReplaceWith = e.Element("replace").Value,
                        };
                })
                .ToList();
        }

        private readonly IEnumerable<QuickBooksPath> _paths;
    }
}
