using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace Toolbelt.AspNetCore.CssLiveReloader.Internals
{
    internal class AddCssFileWatcherMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly ICssFileWatcherService _cssFileWatcherService;

        public AddCssFileWatcherMiddleware(
            RequestDelegate next,
            ICssFileWatcherService cssFileWatcherService)
        {
            _next = next;
            _cssFileWatcherService = cssFileWatcherService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.IsGetCssRequestAndResponce() == false) return;

            var url = context.Request.GetDisplayUrl();
            url = Regex.Replace(url, @"(\?|&)(136bb8a9-b749-47e9-92e7-8b46e4a4f657=\d+&?)", "$1").TrimEnd('?', '&');
            _cssFileWatcherService.TryAddWatch(url);
        }
    }
}
