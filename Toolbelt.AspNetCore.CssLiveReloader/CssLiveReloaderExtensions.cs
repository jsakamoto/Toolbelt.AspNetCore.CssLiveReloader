using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Toolbelt.AspNetCore.CssLiveReloader;
using Toolbelt.AspNetCore.CssLiveReloader.Internals;

namespace Toolbelt.Extensions.DependencyInjection
{
    public static class CssLiveReloaderExtensions
    {
        public static IApplicationBuilder UseCssLiveReload(this IApplicationBuilder app) => UseCssLiveReload(app, null);

        public static IApplicationBuilder UseCssLiveReload(this IApplicationBuilder app, Action<CssLiveReloaderOptions>? configure)
        {
            var defaultStaticFileOptions = app.ApplicationServices.GetService<IOptions<StaticFileOptions>>();
            var hostingEnv = app.ApplicationServices.GetService<IHostingEnvironment>();
            var cssLiveReloadOptions = new CssLiveReloaderOptions
            {
                FileMappings = {
                    new FileMapping(
                        defaultStaticFileOptions.Value.RequestPath,
                        defaultStaticFileOptions.Value.FileProvider ?? hostingEnv.WebRootFileProvider)
                }
            };

            configure?.Invoke(cssLiveReloadOptions);

            var cssFileWatcherService = new CssFileWatcherService(cssLiveReloadOptions);

            app.UseMiddleware<AddCssFileWatcherMiddleware>(cssFileWatcherService);
            app.UseMiddleware<InjectReloaderScriptMiddleware>(cssLiveReloadOptions);
            app.Map("/Toolbelt.AspNetCore.CssLiveReloader/EventSource", builder => builder.Run(context => new CssChangedEventSourceHandler(context, cssFileWatcherService).InvokeAsync()));
            app.Map("/Toolbelt.AspNetCore.CssLiveReloader/WatchRequest", builder => builder.Run(context => CssWatchRequestHandler.InvokeAsync(context, cssFileWatcherService)));
            app.Map(cssLiveReloadOptions.ScriptPath, builder => builder.Run(CssLiveReloaderScriptHandler.InvokeAsync));

            return app;
        }

        public static void Add(this ICollection<FileMapping> mappings, PathString matchUrl, IFileProvider fileProvider)
        {
            mappings.Add(new FileMapping(matchUrl, fileProvider));
        }

        public static void AddPhysicalFileMapping(this ICollection<FileMapping> mappings, PathString matchUrl, string root)
        {
            mappings.Add(new FileMapping(matchUrl, new PhysicalFileProvider(root)));
        }
    }
}
