using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Toolbelt.AspNetCore.CssLiveReloader.Internals
{
    internal class CssLiveReloaderScriptHandler
    {
        public static async Task InvokeAsync(HttpContext context)
        {
            using var resStream = typeof(CssLiveReloaderScriptHandler).Assembly.GetManifestResourceStream("Toolbelt.AspNetCore.CssLiveReloader.script.min.js");
            context.Response.ContentType = "text/jsavascript";
            await resStream.CopyToAsync(context.Response.Body);
        }
    }
}
