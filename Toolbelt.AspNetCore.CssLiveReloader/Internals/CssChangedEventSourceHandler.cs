using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Toolbelt.AspNetCore.CssLiveReloader.Internals
{
    internal class CssChangedEventSourceHandler
    {
        private readonly HttpContext _context;

        private readonly CssFileWatcherService _cssFileWatcherService;

        public CssChangedEventSourceHandler(HttpContext context, CssFileWatcherService cssFileWatcherService)
        {
            this._context = context;
            this._cssFileWatcherService = cssFileWatcherService;
        }

        public async Task InvokeAsync()
        {
            _cssFileWatcherService.CssFileChanged += CssFileWatcherService_CssFileChanged;

            _context.Response.ContentType = "text/event-stream";

            var bytes = Encoding.ASCII.GetBytes($"event: connected\n\n");
            await _context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            await _context.Response.Body.FlushAsync();

            var waiter = new TaskCompletionSource<object?>();
            _context.RequestAborted.Register(() => waiter.TrySetResult(null));
            await waiter.Task;

            _cssFileWatcherService.CssFileChanged -= CssFileWatcherService_CssFileChanged;
        }

        private void CssFileWatcherService_CssFileChanged(object sender, CssFileChangedEventArgs e)
        {
            var bytes = Encoding.ASCII.GetBytes($"event: css-changed\ndata: {e.Url}\n\n");
            _context.Response.Body.WriteAsync(bytes, 0, bytes.Length)
                .ContinueWith(t => _context.Response.Body.FlushAsync());
        }
    }
}
