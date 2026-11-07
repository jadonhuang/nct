using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using Nct.EmbeddedResources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nct.Ioc;

namespace Nct.AspNetCore.EmbeddedResources
{
    public class EmbeddedResourceFileProvider : IFileProvider
    {
        private bool _isInitialized;
        private HashSet<string> _ignorFileExts = null;
        private readonly Lazy<IEmbeddedResourceManager> _embeddedResourceManager;
        private readonly IIocResolver _iocResolver;
        public EmbeddedResourceFileProvider(IIocResolver iocResolver)
        {
            _iocResolver = iocResolver;
            _ignorFileExts = new HashSet<string> { "cshtml", "config" };
            _embeddedResourceManager = new Lazy<IEmbeddedResourceManager>(
              _iocResolver.Resolve<IEmbeddedResourceManager>,
             true
         );
        }


    

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return null;
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            if (!IsInitialized())
            {
                return new NotFoundFileInfo(subpath);
            }

            var filename = Path.GetFileName(subpath);
            var resource = _embeddedResourceManager.Value.GetResource(subpath);
            if (resource == null || IsIgnoredFile(resource))
            {
                return new NotFoundFileInfo(subpath);
            }

            return new EmbeddedResourceFileInfo(resource, filename);
        }


        protected virtual bool IsIgnoredFile(EmbeddedResourceItem resource)
        {
            return resource.FileExtension != null && _ignorFileExts.Contains(resource.FileExtension);
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }


        private bool IsInitialized()
        {
            if (_isInitialized)
            {
                return true;
            }

            _isInitialized = _iocResolver.IsRegistered<IEmbeddedResourceManager>();
            return _isInitialized;
        }
    }
}
