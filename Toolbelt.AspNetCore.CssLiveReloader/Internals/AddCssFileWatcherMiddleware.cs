using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Toolbelt.AspNetCore.CssLiveReloader.Internals
{
    internal class AddCssFileWatcherMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly CssFileWatcherService _cssFileWatcherService;

        private readonly CssLiveReloaderOptions _options;

        public AddCssFileWatcherMiddleware(
            RequestDelegate next,
            CssFileWatcherService cssFileWatcherService,
            CssLiveReloaderOptions options)
        {
            _next = next;
            _cssFileWatcherService = cssFileWatcherService;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            var httpStatus = (HttpStatusCode)context.Response.StatusCode;
            if (httpStatus != HttpStatusCode.OK && httpStatus != HttpStatusCode.NotModified) return;
            if (context.Response.ContentType.Split(',', ';', ' ').FirstOrDefault() != "text/css") return;
            if (!HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsHead(context.Request.Method)) return;

            var requestPath = context.Request.Path;
            var url = context.Request.GetDisplayUrl();
            url = Regex.Replace(url, @"(\?|&)(136bb8a9-b749-47e9-92e7-8b46e4a4f657=\d+&?)", "$1").TrimEnd('?', '&');

            _cssFileWatcherService.TryAddWatch(url, () =>
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
        }
    }
}
