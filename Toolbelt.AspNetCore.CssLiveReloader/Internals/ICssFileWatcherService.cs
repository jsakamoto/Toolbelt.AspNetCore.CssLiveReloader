using System;

namespace Toolbelt.AspNetCore.CssLiveReloader.Internals
{
    internal interface ICssFileWatcherService
    {
        event EventHandler<CssFileChangedEventArgs>? CssFileChanged;

        void TryAddWatch(string url, bool timeStampCheck = false, DateTime timeStamp = default);
    }
}
