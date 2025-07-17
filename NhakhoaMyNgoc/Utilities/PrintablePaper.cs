using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.Utilities
{
    public abstract class PrintablePaper
    {
        public static string RESOURCE_PATH => Path.Combine(Config.full_path, "Templates");
        public bool Landscape { get; set; }
        public PrintablePaper() => IOUtil.CopyDirectory(RESOURCE_PATH, Path.GetTempPath());
        public abstract string GetTemplateName();
        public abstract object GetFileName();
        public abstract void Edit(ref string templateSrc);
        public void Render()
        {
            string templateSrc = File.ReadAllText(Path.Combine(RESOURCE_PATH, GetTemplateName() + ".html"));
            Edit(ref templateSrc);
            File.WriteAllText(Path.Combine(Path.GetTempPath(), GetFileName().ToString() + ".html"), templateSrc);
        }
    }
}
