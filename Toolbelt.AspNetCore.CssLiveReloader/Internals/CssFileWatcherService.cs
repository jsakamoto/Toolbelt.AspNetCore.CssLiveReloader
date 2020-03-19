using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Toolbelt.AspNetCore.CssLiveReloader.Internals
{
    internal class CssFileWatcherService : IDisposable
    {
        private readonly ConcurrentDictionary<string, string> _watchTargetUrlToPath = new ConcurrentDictionary<string, string>();

        private readonly Dictionary<string, FileSystemWatcher> _fileSystemWatchers = new Dictionary<string, FileSystemWatcher>();

        public event EventHandler<CssFileChangedEventArgs> CssFileChanged;

        public CssFileWatcherService()
        {
        }

        public void TryAddWatch(string url, Func<string> getPyhisicalPath)
        {
            if (this._watchTargetUrlToPath.ContainsKey(url)) return;

            var pyhisicalPath = getPyhisicalPath();
            if (!this._watchTargetUrlToPath.TryAdd(url, pyhisicalPath)) return;

            if (string.IsNullOrEmpty(pyhisicalPath)) return;

            var targetDir = Path.GetDirectoryName(pyhisicalPath);
            var filter = "*" + Path.GetExtension(pyhisicalPath);
            var key = Path.Combine(targetDir, filter);

            lock (this._fileSystemWatchers)
            {
                if (this._fileSystemWatchers.ContainsKey(key)) return;
                var watcher = new FileSystemWatcher();
                this._fileSystemWatchers.Add(key, watcher);
                watcher.Path = targetDir;
                watcher.Filter = filter;
                watcher.Changed += Watcher_Changed;
                watcher.Created += Watcher_Changed;
                watcher.Renamed += Watcher_Changed;
                watcher.EnableRaisingEvents = true;
            }
        }

        public void Dispose()
        {
            lock (_fileSystemWatchers)
            {
                foreach (var watcher in this._fileSystemWatchers.Values)
                {
                    watcher.EnableRaisingEvents = false;
                    watcher.Changed -= Watcher_Changed;
                    watcher.Created -= Watcher_Changed;
                    watcher.Renamed -= Watcher_Changed;
                    watcher.Dispose();
                }
                this._fileSystemWatchers.Clear();
            }
        }

        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            foreach (var url in _watchTargetUrlToPath.Where(entry => entry.Value == e.FullPath).Select(entry => entry.Key))
            {
                this.CssFileChanged?.Invoke(this, new CssFileChangedEventArgs(url));
            }
        }
    }
}
