using System.Collections.Generic;

namespace Toolbelt.AspNetCore.CssLiveReloader
{
    public class CssLiveReloaderOptions
    {
        public string ScriptPath { get; set; } = "/Toolbelt.AspNetCore.CssLiveReloader/script.min.js";

        public List<FileMapping> FileMappings { get; } = new List<FileMapping>();
    }
}
