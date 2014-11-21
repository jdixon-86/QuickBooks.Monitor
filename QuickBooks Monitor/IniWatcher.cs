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
using FileStateEnforcer;

namespace QuickBooks_Monitor
{
    public partial class IniWatcher : ServiceBase
    {
        public IniWatcher()
        {
            InitializeComponent();
            ServiceName = "Quickbooks Monitor";
        }

        protected override void OnStart(string[] args)
        {
            var configPath = args.FirstOrDefault() ?? string.Format(
                "{0}/config.xml",
                System.Reflection.Assembly.GetEntryAssembly().Location);

            var settings = Settings.ImportSettings(XDocument.Load(configPath));

            _enforcers.AddRange(settings.Select(x => new Enforcer(x)));

            foreach (var enforcer in _enforcers)
                enforcer.Begin();
        }

        protected override void OnStop()
        {
            foreach (var enforcer in _enforcers)
                enforcer.Dispose();

            _enforcers.Clear();
        }

        private List<Enforcer> _enforcers = new List<Enforcer>();
    }
}
