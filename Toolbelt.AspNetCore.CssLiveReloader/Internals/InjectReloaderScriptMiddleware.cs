using System.IO;
using System.Text;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html;
using AngleSharp.Html.Parser;
using Microsoft.AspNetCore.Http;

namespace Toolbelt.AspNetCore.CssLiveReloader.Internals
{
    internal class InjectReloaderScriptMiddleware
    {
        private readonly RequestDelegate _next;

        private readonly CssLiveReloaderOptions _options;

        public InjectReloaderScriptMiddleware(RequestDelegate next, CssLiveReloaderOptions options)
        {
            _next = next;
            _options = options;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var filter = new FilterStream(context);

            try
            {
                await _next(context);

                if (filter.IsCaptured())
                {
                    filter.MemoryStream.Seek(0, SeekOrigin.Begin);
                    var parser = new HtmlParser();
                    using var doc = parser.ParseDocument(filter.MemoryStream);

                    doc.Body.Insert(AdjacentPosition.BeforeEnd, $"<script src=\"{_options.ScriptPath}\" type=\"text/javascript\"></script>");

                    filter.MemoryStream.SetLength(0);
                    var encoding = Encoding.UTF8;
                    using var writer = new StreamWriter(filter.MemoryStream, bufferSize: -1, leaveOpen: true, encoding: encoding) { AutoFlush = true };
                    doc.ToHtml(writer, new PrettyMarkupFormatter());

                    context.Response.ContentLength = filter.MemoryStream.Length;
                    filter.MemoryStream.Seek(0, SeekOrigin.Begin);
                    await filter.MemoryStream.CopyToAsync(filter.OriginalStream);
                }
            }
            finally
            {
                filter.RevertResponseBodyHooking();
            }
        }
    }
}
