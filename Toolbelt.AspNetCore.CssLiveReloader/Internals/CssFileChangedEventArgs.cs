using System;

namespace Toolbelt.AspNetCore.CssLiveReloader.Internals
{
    internal class CssFileChangedEventArgs : EventArgs
    {
        public string Url { get; }

        public CssFileChangedEventArgs(string url)
        {
            Url = url;
        }
    }
}
