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

            var httpStatus = (HttpStatusCode)context.Response.StatusCode;
            if (httpStatus != HttpStatusCode.OK && httpStatus != HttpStatusCode.NotModified) return;
            if (!HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsHead(context.Request.Method)) return;
            if ((context.Response.ContentType ?? "").Split(',', ';', ' ').FirstOrDefault() != "text/css") return;

            var url = context.Request.GetDisplayUrl();
            url = Regex.Replace(url, @"(\?|&)(136bb8a9-b749-47e9-92e7-8b46e4a4f657=\d+&?)", "$1").TrimEnd('?', '&');
            _cssFileWatcherService.TryAddWatch(url);
        }
    }
}
