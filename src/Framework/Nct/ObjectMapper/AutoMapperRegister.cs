using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Linq;
using Nct.Modularity;
using Nct.Ioc;
using Castle.MicroKernel.Registration;

namespace Nct.ObjectMapper
{
    public static class AutoMapperRegister
    {
        public static void RegisteAll(IReadOnlyList<Type> allTypes)
        {


            var mapperConfig = new MapperConfiguration(x =>
            {

                x.ShouldMapMethod = (m) =>
                {

                    return false;

                };

                foreach (var type in allTypes)
                {

                    var typeInfo = type.GetTypeInfo();
                    if (!(typeInfo.IsClass &&
                        !typeInfo.IsAbstract &&
                        !typeInfo.IsGenericType))
                    {
                        continue;
                    }


                    if (typeof(Profile).IsAssignableFrom(type))
                    {
                        x.AddProfile(type);
                        continue;
                    }

                    var attrs = type.GetCustomAttributes();

                    foreach (Attribute attr in attrs)
                    {
                        if (attr is MapToAttribute mapToAttr)
                        {
                            x.CreateMap(type, mapToAttr.TargetType);

                        }
                        else if (attr is MapFromAttribute mapFromAttr)
                        {
                            x.CreateMap(mapFromAttr.SourceType, type);

                        }
                        else if (attr is MapAttribute mapAttr)
                        {
                            x.CreateMap(mapAttr.TargetType, type).ReverseMap();


                        }

                    }


                }



            });


            var mapper = mapperConfig.CreateMapper();
            IocManager.Instance.Container.Register(Component.For<IMapper>().Instance(mapper).LifestyleSingleton());


        }


    }
}
