using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nct.EmbeddedResources
{
    public  interface IEmbeddedResourceManager
    {
        EmbeddedResourceItem GetResource(string resourcePath);


        IEnumerable<EmbeddedResourceItem> GetResources(string resourcePath);

        void SetResourceAssemblies(List<Assembly> assemblies);
    }
}
