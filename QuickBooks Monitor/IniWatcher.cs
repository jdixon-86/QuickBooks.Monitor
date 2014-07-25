using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Xml.Linq;

namespace QuickBooks_Monitor
{
    public partial class IniWatcher : ServiceBase
    {
        public IniWatcher()
        {
            InitializeComponent();
            ServiceName = "Quickbooks Monitor";
            var fileChangerWorker = new Thread(() =>
                {
                    for (;;)
                    {
                        var fileInfo = _changedFiles.Take();

                        var oldText = File.ReadAllLines(fileInfo.FullPath);

                        File.WriteAllLines(
                            fileInfo.FullPath,
                            oldText.Select(x => x.Trim() == fileInfo.LookFor ? fileInfo.ReplaceWith.Trim() : x));
                    }
                })
                { IsBackground = true, };

            fileChangerWorker.Start();
        }

        protected override void OnStart(string[] args)
        {
            var configPath = args.FirstOrDefault() ?? string.Format(
                "{0}/config.xml",
                System.Reflection.Assembly.GetEntryAssembly().Location);

            var settings = new QBMonitorSettings(XDocument.Load(configPath));

            _watchers.AddRange(settings.Instances.Select(x =>
                {
                    var filename = Path.GetFileName(x.FullPath);

                    return new FileSystemWatcher(x.Path, x.Filename);
                }));

            foreach (var item in _watchers.Zip(settings.Instances, (x, y) => new { Watcher = x, Settings = y }))
            {
                item.Watcher.BeginInit();
                item.Watcher.Changed += (o, e) =>
                    {
                        _changedFiles.Add(item.Settings);
                    };

                item.Watcher.EnableRaisingEvents = true;
            }
        }

        protected override void OnStop()
        {
            foreach (var watcher in _watchers)
                watcher.Dispose();

            _watchers.Clear();
        }

        private List<FileSystemWatcher> _watchers = new List<FileSystemWatcher>();
        private BlockingCollection<QuickBooksPath> _changedFiles = new BlockingCollection<QuickBooksPath>();
    }
}
