using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Toolbelt.AspNetCore.CssLiveReloader.Internals
{
    internal static class HttpContextExtensions
    {
        public static bool IsGetCssRequestAndResponce(this HttpContext context)
        {
            var httpStatus = (HttpStatusCode)context.Response.StatusCode;
            if (httpStatus != HttpStatusCode.OK && httpStatus != HttpStatusCode.NotModified) return false;
            if (!HttpMethods.IsGet(context.Request.Method) && !HttpMethods.IsHead(context.Request.Method)) return false;
            if ((context.Response.ContentType ?? "").Split(',', ';', ' ').FirstOrDefault() != "text/css") return false;

            return true;
        }
    }
}
