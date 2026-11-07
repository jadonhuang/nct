using Nct.Cache.Configuration;
using Nct.Extentions;
using Nct.Ioc;
using Nct.Jobs;
using System;
using System.Collections.Generic;

namespace Nct.Configuration
{
    public class StartupConfiguration : Dictionary<string, object>
    {

        public StartupConfiguration()
        {

            Module = new ModuleConfigurations(this);
            Caching = new CacheConfiguration();
            ServiceReplaceActions = new Dictionary<Type, Action>();
            Job = new JobConfiguration();
         
        }

        public CacheConfiguration Caching { get; private set; }

        public JobConfiguration Job { get; private set; }

        public ModuleConfigurations Module { get; private set; }

        public Dictionary<Type, Action> ServiceReplaceActions { get; private set; }


        public void ReplaceService(Type type, Action replaceAction)
        {

            ServiceReplaceActions[type] = replaceAction;
        }


        public T Get<T>()
        {
            return (T)this.GetOrAdd(typeof(T).FullName, () =>
            {

                return IocManager.Instance.Resolve<T>();

            });
        }
    }
}
