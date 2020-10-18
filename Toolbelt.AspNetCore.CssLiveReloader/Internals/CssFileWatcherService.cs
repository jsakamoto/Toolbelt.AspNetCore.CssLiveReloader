using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Toolbelt.AspNetCore.CssLiveReloader.Internals
{
    internal class CssFileWatcherService : IDisposable
    {
        private readonly ConcurrentDictionary<string, string> _watchTargetUrlToPath = new ConcurrentDictionary<string, string>();

        private readonly Dictionary<string, FileSystemWatcher> _fileSystemWatchers = new Dictionary<string, FileSystemWatcher>();

        private readonly CssLiveReloaderOptions _options;

        public event EventHandler<CssFileChangedEventArgs> CssFileChanged;

        public CssFileWatcherService(CssLiveReloaderOptions cssLiveReloadOptions)
        {
            this._options = cssLiveReloadOptions;
        }

        public void TryAddWatch(string url, bool timeStampCheck = false, DateTime timeStamp = default)
        {
            var requestPath = PathString.FromUriComponent(new Uri(url));
            this.TryAddWatch(url, () =>
            {
                lock (this._options.FileMappings)
                {
                    foreach (var fileMapping in _options.FileMappings)
                    {
                        if (!requestPath.StartsWithSegments(fileMapping.MatchUrl, out var subpath)) continue;
                        var fileInfo = fileMapping.FileProvider.GetFileInfo(subpath.Value);
                        if (!fileInfo.Exists) continue;
                        return fileInfo.PhysicalPath;
                    }
                    return null;
                }
            });

            if (timeStampCheck == false) return;
            if (this._watchTargetUrlToPath.TryGetValue(url, out var physicalPath) == false) return;

            var fileTimeStamp = File.GetLastWriteTimeUtc(physicalPath);
            if (timeStamp < fileTimeStamp)
            {
                this.CssFileChanged?.Invoke(this, new CssFileChangedEventArgs(url));
            }
        }

        private void TryAddWatch(string url, Func<string> getPyhisicalPath)
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
