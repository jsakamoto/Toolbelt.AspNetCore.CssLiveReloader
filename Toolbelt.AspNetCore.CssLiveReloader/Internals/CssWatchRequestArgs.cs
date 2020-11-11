using System;

namespace Toolbelt.AspNetCore.CssLiveReloader.Internals
{
    internal class CssWatchRequestArgs
    {
        public bool ConnectedOnce { get; set; }

        public DateTime LastReloadedTime { get; set; }

        public string[]? Hrefs { get; set; }
    }
}
