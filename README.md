# ASP.NET Core CSS Live Reloader [![NuGet Package](https://img.shields.io/nuget/v/Toolbelt.AspNetCore.CssLiveReloader.svg)](https://www.nuget.org/packages/Toolbelt.AspNetCore.CssLiveReloader/)

## Summary

This is a yet another CSS live reloader for ASP.NET Core App as an ASP.NET Core middleware.

![fig.1](https://raw.githubusercontent.com/jsakamoto/Toolbelt.AspNetCore.CssLiveReloader/master/.assets/fig1.gif)

There are already many live reloading solutions.  
However, instead of this middleware works for only CSS files, it reload styles **more smoothly** rather than other solutions. 

## How to use

1. Add `Toolbelt.AspNetCore.CssLiveReloader` NuGet package reference to your ASP.NET Core Web App project.

```shell
dotnet add package Toolbelt.AspNetCore.CssLiveReloader
```

2. After installing this package, **what you have to do is just add one line `app.UseCssLiveReload();`** in your startup code, like this:

```csharp
...
using Toolbelt.Extensions.DependencyInjection; // <- Add this, and...
...
public class Startup
{
  ...
  public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
  {
    ...
    if (env.IsDevelopment())
    {
      ...
      app.UseCssLiveReload(); // 👈 Add this!
```

That's all!

## Release Notes

[Release Notes](https://github.com/jsakamoto/Toolbelt.AspNetCore.CssLiveReloader/blob/master/RELEASE-NOTES.txt)

## License

[Mozilla Public License Version 2.0](https://github.com/jsakamoto/Toolbelt.AspNetCore.CssLiveReloader/blob/master/LICENSE)

