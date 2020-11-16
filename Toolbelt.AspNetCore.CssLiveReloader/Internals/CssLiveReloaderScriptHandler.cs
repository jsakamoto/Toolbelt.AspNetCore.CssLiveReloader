using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Toolbelt.AspNetCore.CssLiveReloader.Internals
{
    internal class CssLiveReloaderScriptHandler
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            const string resourceName =
#if DEBUG
                "Toolbelt.AspNetCore.CssLiveReloader.script.js";
#else
                "Toolbelt.AspNetCore.CssLiveReloader.script.min.js";
#endif
            using var resStream = typeof(CssLiveReloaderScriptHandler).Assembly.GetManifestResourceStream(resourceName);
            context.Response.ContentType = "text/javascript";
            await resStream.CopyToAsync(context.Response.Body);
        }
    }
}
