using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;

namespace Toolbelt.AspNetCore.CssLiveReloader
{
    public class FileMapping
    {
        public PathString MatchUrl { get; }

        public IFileProvider FileProvider { get; }

        public FileMapping(PathString matchUrl, IFileProvider fileProvider)
        {
            MatchUrl = matchUrl;
            FileProvider = fileProvider;
        }
    }
}
