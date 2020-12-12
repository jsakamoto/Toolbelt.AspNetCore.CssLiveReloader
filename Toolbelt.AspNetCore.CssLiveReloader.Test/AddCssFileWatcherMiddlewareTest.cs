using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using Toolbelt.AspNetCore.CssLiveReloader.Internals;

namespace Toolbelt.AspNetCore.CssLiveReloader.Test
{
    public class AddCssFileWatcherMiddlewareTest
    {
        private static readonly RequestDelegate next = (HttpContext _) => Task.CompletedTask;

        [TestCase("GET", "http://localhost/foo/bar.css", 200, "text/css;charset=utf-8")]
        [TestCase("HEAD", "http://localhost/foo/bar.css", 200, "text/css;charset=utf-8")]
        public async Task InvokeAsync_url_should_be_added_Test(string method, string url, int status, string? contentType)
        {
            var httpContext = CreateHttpContext(method, url, status, contentType);

            var fileWatcherServiceMock = new Mock<ICssFileWatcherService>();
            var middleWare = new AddCssFileWatcherMiddleware(next, fileWatcherServiceMock.Object);
            await middleWare.InvokeAsync(httpContext);

            fileWatcherServiceMock.Verify(m =>
                m.TryAddWatch(
                    It.Is<string>(url => url == "http://localhost/foo/bar.css"),
                    It.Is<bool>(timeStampCheck => timeStampCheck == false),
                    It.Is<DateTime>(timeStamp => timeStamp == DateTime.MinValue)),
                Times.Once);
        }

        [TestCase("POST", "http://localhost/foo/bar.css", 200, "text/css;charset=utf-8")] // method is not "GET" and "HEAD".
        [TestCase("GET", "http://localhost/foo/bar.css", 404, "text/css;charset=utf-8")] // status code is not 200.
        [TestCase("GET", "http://localhost/foo/bar.css", 200, "application/json;charset=utf-8")] // content type is not "text/css".
        [TestCase("GET", "http://localhost/foo/bar.css", 200, null)] // content type is null.
        public async Task InvokeAsync_url_should_NOT_be_added_Test(string method, string url, int status, string? contentType)
        {
            var httpContext = CreateHttpContext(method, url, status, contentType);

            var fileWatcherServiceMock = new Mock<ICssFileWatcherService>();
            var middleWare = new AddCssFileWatcherMiddleware(next, fileWatcherServiceMock.Object);
            await middleWare.InvokeAsync(httpContext);

            fileWatcherServiceMock.Verify(m =>
                m.TryAddWatch(It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<DateTime>()),
                Times.Never);
        }

        private static DefaultHttpContext CreateHttpContext(string method, string url, int status, string? contentType)
        {
            var uri = new Uri(url);
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Method = method;
            httpContext.Request.Scheme = uri.Scheme;
            httpContext.Request.Host = new HostString(uri.Host);
            httpContext.Request.Path = new PathString(uri.PathAndQuery);
            httpContext.Response.StatusCode = status;
            httpContext.Response.ContentType = contentType;
            return httpContext;
        }
    }
}