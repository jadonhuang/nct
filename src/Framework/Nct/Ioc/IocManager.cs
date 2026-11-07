using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Castle.Windsor.Proxy;
using System;
using System.Collections.Generic;
using System.Reflection;
namespace Nct.Ioc
{
    public class IocManager : IIocManager
    {
        public static IocManager Instance { get; private set; }



        public IWindsorContainer Container { get; set; }

        private static readonly ProxyGenerator ProxyGeneratorInstance = new ProxyGenerator();


        static IocManager()
        {

            Instance = new IocManager();
        }




        public IocManager()
        {
            Container = new WindsorContainer(new DefaultProxyFactory(ProxyGeneratorInstance));

            Container.Register(
               Component
                   .For<IocManager, IIocManager, IIocRegistrar, IIocResolver>()
                   .Instance(this));

        }

        public bool IsRegistered(Type type)
        {

            return Container.Kernel.HasComponent(type);
        }


        public bool IsRegistered<TType>()
        {
            return IsRegistered(typeof(TType));
        }



        public object Resolve(Type type)
        {

            return Container.Resolve(type);

        }


        public T Resolve<T>()
        {
            return Container.Resolve<T>();
        }

        public T ResolveOrNull<T>()
        {
            if (IsRegistered<T>())
            {
                return Resolve<T>();
            }

            return default;
        }


        public T Resolve<T>(Type type)
        {
            return (T)Resolve(type);
        }

        public T Resolve<T>(IDictionary<string, object> args)
        {

            return Container.Resolve<T>(args);

        }



        public void RegisterAssemblyByConvention(Assembly assembly)
        {


            Container.Register(
                Classes.FromAssembly(assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<ITransientDependency>()
                    .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
                    .WithService.Self()
                    .WithService.DefaultInterfaces()  //the type name must contains interface name
                    .LifestyleTransient()
                );


            Container.Register(
                 Classes.FromAssembly(assembly)
                     .IncludeNonPublicTypes()
                     .BasedOn<ISingletonDependency>()
                     .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
                     .WithService.Self()
                     .WithService.DefaultInterfaces()
                     .LifestyleSingleton()
                 );


            Container.Register(
                Classes.FromAssembly(assembly)
                    .IncludeNonPublicTypes()
                    .BasedOn<IInterceptor>()
                    .If(type => !type.GetTypeInfo().IsGenericTypeDefinition)
                    .WithService.Self()
                    .LifestyleTransient()
                );



        }


        private static ComponentRegistration<T> ApplyLifestyle<T>(ComponentRegistration<T> registration, DependencyLifeStyle lifeStyle)
            where T : class
        {
            return lifeStyle switch
            {
                DependencyLifeStyle.Transient => registration.LifestyleTransient(),
                DependencyLifeStyle.Singleton => registration.LifestyleSingleton(),
                _ => registration,
            };
        }

        public void Register<T>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where T : class
        {
           
            Container.Register(ApplyLifestyle(Component.For<T>(), lifeStyle));

        }

        public void Register(Type type, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {

            Container.Register(ApplyLifestyle(Component.For(type), lifeStyle));

        }

        public void Register<TType, TImpl>(DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton) where TType : class
             where TImpl : class, TType
        {

            Container.Register(ApplyLifestyle(Component.For<TType, TImpl>().ImplementedBy<TImpl>(), lifeStyle));


        }

        public void Register(Type type, Type impl, DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {

            Container.Register(ApplyLifestyle(Component.For(type, impl).ImplementedBy(impl), lifeStyle));

        }

        public void Dispose()
        {
            Container.Dispose();
        }

        public void Release(object obj)
        {
            Container.Release(obj);
        }


    }
}
