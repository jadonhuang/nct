using Nct.Ioc;
using System;


namespace Nct.Configuration
{
   
    public static class StartupConfigurationExtensions
    {
       
        public static void ReplaceService(this StartupConfiguration configuration, Type type, Type impl, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            configuration.ReplaceService(type, () =>
            {
                IocManager.Instance.Register(type, impl, lifeStyle);
            });
        }

     
        public static void ReplaceService<TType, TImpl>(this StartupConfiguration configuration, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            configuration.ReplaceService(typeof(TType), () =>
            {
                IocManager.Instance.Register<TType, TImpl>(lifeStyle);
            });
        }


   
    }
}