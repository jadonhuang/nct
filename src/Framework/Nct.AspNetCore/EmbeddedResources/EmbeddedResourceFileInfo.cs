using Microsoft.Extensions.FileProviders;
using Nct.EmbeddedResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Nct.AspNetCore.EmbeddedResources
{
    public class EmbeddedResourceFileInfo : IFileInfo
    {
        public bool Exists => true;

        public bool IsDirectory => false;

        public DateTimeOffset LastModified => _resource.LastModifiedUtc;

        public long Length => _resource.Length;

        public string Name { get; }

        public string PhysicalPath => null;

  
        private readonly EmbeddedResourceItem _resource;

        public EmbeddedResourceFileInfo(EmbeddedResourceItem resource,string name)
        {
            Name = name;
            _resource = resource;
        }

        public Stream CreateReadStream()
        {
            return _resource.GetStream();

        }

    }
}
