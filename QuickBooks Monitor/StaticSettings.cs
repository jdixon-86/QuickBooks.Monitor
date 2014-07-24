using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace QuickBooks_Monitor.config
{
    public static class StaticSettings
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StaticSettings));

        public static List<QuickBooksPaths> paths;

        public static void GetSettings()
        {
            try
            {
                string path = string.Format("{0}/config.xml", System.Reflection.Assembly.GetEntryAssembly().Location);

                log.DebugFormat("Loading settings from {0}", path);
                XDocument xDoc = XDocument.Load(path);

                var root = from r in xDoc.Element("watcher").Elements("data")
                           select r;

                // Replace settings
                paths = new List<QuickBooksPaths>();
                foreach (var e in root)
                {
                    // Get the elements
                    QuickBooksPaths newPath = new QuickBooksPaths();
                    newPath.Path = e.Element("path").Value;
                    newPath.Filename = e.Element("filename").Value;
                    newPath.LookFor = e.Element("line").Value;
                    newPath.LookFor = e.Element("replace").Value;

                    // Add to our list
                    paths.Add(newPath);
                }

            }
            catch (Exception ex)
            {
                log.ErrorFormat("Error retrieving settings. Error: {0}", ex.ToString());
                throw;
            }
        }
    }
}
