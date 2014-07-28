using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FileStateEnforcer
{
    public class Enforcer : IDisposable
    {
        public Enforcer(LineRule rule)
        {
            _rule = rule;

            var filename = Path.GetFileName(_rule.FullPath);

            _watcher = new FileSystemWatcher(_rule.Directory, _rule.Filename);
            _watcher.BeginInit();
            _watcher.Changed += (o, e) =>
            {
                _fileChanges.Add(_rule);
            };
        }

        public void Begin(Action action = null)
        {
            var workerThread = new Thread((object obj) =>
                {
                    var token = (CancellationToken)obj;
                    while (!token.IsCancellationRequested)
                    {
                        var fileInfo = _fileChanges.Take();

                        var oldText = File.ReadAllLines(fileInfo.FullPath);

                        File.WriteAllLines(
                            fileInfo.FullPath,
                            oldText.Select(x => x.Contains(fileInfo.Search) ? fileInfo.Rule.Trim() : x));

                        if (action != null) action();
                    }
                })
                { IsBackground = true, };

            workerThread.Start(_tokenSource.Token);

            _watcher.EnableRaisingEvents = true;
        }

        public void End()
        {
            _tokenSource.Cancel();
            _watcher.EnableRaisingEvents = false;
        }

        private readonly LineRule _rule;
        private readonly BlockingCollection<LineRule> _fileChanges = new BlockingCollection<LineRule>();
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private FileSystemWatcher _watcher;

        #region IDisposable Members

        public void Dispose()
        {
            End();
            _tokenSource.Dispose();
            _watcher.Dispose();
        }

        #endregion
    }
}
