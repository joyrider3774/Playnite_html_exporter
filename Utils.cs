using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HtmlExporterPlugin
{
    static class Utils
    {
        static public bool IsRootDrive(string Path)
        {
            DirectoryInfo d = new DirectoryInfo(Path);
            return (d.Parent == null);
        }
    }
}
