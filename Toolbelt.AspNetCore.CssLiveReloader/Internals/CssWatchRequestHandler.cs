using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Toolbelt.AspNetCore.CssLiveReloader.Internals
{
    internal class CssWatchRequestHandler
    {
        public static async Task InvokeAsync(HttpContext context, CssFileWatcherService cssFileWatcherService)
        {
            if (!HttpMethods.IsPost(context.Request.Method))
            {
                context.Response.StatusCode = (int)HttpStatusCode.MethodNotAllowed;
                return;
            }
            if (!context.Request.ContentType.Split(';').Select(s => s.Trim()).Any(s => s == "application/json"))
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return;
            }

            var hrefs = await JsonSerializer.DeserializeAsync<string[]>(context.Request.Body);
            foreach (var href in hrefs)
            {
                cssFileWatcherService.TryAddWatch(href);
            }

            context.Response.StatusCode = (int)HttpStatusCode.NoContent;
        }
    }
}
