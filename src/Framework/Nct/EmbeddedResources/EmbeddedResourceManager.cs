using Nct.Ioc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace Nct.EmbeddedResources
{
    public class EmbeddedResourceManager : IEmbeddedResourceManager, ISingletonDependency
    {

        private readonly Lazy<Dictionary<string, EmbeddedResourceItem>> _resources;

        private List<Assembly> _resourceAssemblies = null;


        public EmbeddedResourceManager()
        {
         
            _resources = new Lazy<Dictionary<string, EmbeddedResourceItem>>(
              CreateResourcesDictionary,
              true
          );

        }


        public void SetResourceAssemblies(List<Assembly> assemblies)
        {
            _resourceAssemblies = assemblies;
        }

        public EmbeddedResourceItem GetResource(string resourcePath)
        {

            var path = EmbeddedResourceHelper.EncodeAsResourcesPath(resourcePath);
            EmbeddedResourceItem item;
            _resources.Value.TryGetValue(path, out item);
            return item;

        }

        public IEnumerable<EmbeddedResourceItem> GetResources(string resourcePath)
        {
            var encodedPath = EmbeddedResourceHelper.EncodeAsResourcesSubPath(resourcePath); 
            return _resources.Value.Where(k => k.Key.StartsWith(encodedPath)).Select(d => d.Value);
        }

        private Dictionary<string, EmbeddedResourceItem> CreateResourcesDictionary()
        {
            var resources = new Dictionary<string, EmbeddedResourceItem>(StringComparer.OrdinalIgnoreCase);
            if (_resourceAssemblies != null)
            {
                foreach (var asm in _resourceAssemblies)
                {

                    var asmNamespace = asm.GetName().Name; //假定命名空间就是名称
                    var resourceNames = asm.GetManifestResourceNames();
                    foreach (var resourceName in resourceNames)
                    {
                        using (var stream = asm.GetManifestResourceStream(resourceName))
                        {
                            var relativePath = resourceName.Substring(asmNamespace.Length + 1);
                            var resItem = new EmbeddedResourceItem(resourceName,
                              stream.Length,
                               asm
                           );

                            resources[relativePath] = resItem;

                        }

                    }

                }
            }

            return resources;
        }



       
    }
}
