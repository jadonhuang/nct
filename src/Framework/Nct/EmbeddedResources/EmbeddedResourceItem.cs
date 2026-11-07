using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nct.EmbeddedResources
{
    public class EmbeddedResourceItem
    {
       
        public string Name { get; }
    
        public long Length { get;}

        public string FileExtension { get; }

        public Assembly Assembly { get; set; }

        public DateTime LastModifiedUtc { get; }

        public Stream GetStream() {

           return Assembly.GetManifestResourceStream(Name);

        }

        internal EmbeddedResourceItem(string name,long length, Assembly assembly)
        {
            Length = length;
            Name = name;
            FileExtension = GetFileExtension(name);
            Assembly = assembly;
            LastModifiedUtc = !string.IsNullOrEmpty(Assembly.Location)
              ? new FileInfo(Assembly.Location).LastWriteTimeUtc
              : DateTime.UtcNow;

        }

        private static string GetFileExtension(string fileName)
        {
            if (!fileName.Contains("."))
            {
                return null;
            }

            return fileName.Substring(fileName.LastIndexOf(".", StringComparison.Ordinal) + 1);
        }
    }
}
