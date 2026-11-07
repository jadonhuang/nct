using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Nct.Configuration;

namespace Nct.Helpers
{
    public static class PathHelper
    {


        private static string RemovePrefix(string path)
        {

            return path.TrimStart('~', '/');


        }

        public static string WebPath(string path)
        {
            path = RemovePrefix(path);
            return Path.GetFullPath(Path.Combine(GlobalConfiguration.WebRootPath, path));
        }

        public static string ContentPath(string path)
        {
            path = RemovePrefix(path);
            return Path.GetFullPath(Path.Combine(GlobalConfiguration.ContentRootPath, path));
        }

    }
}
