using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Ioc
{
    public class IocHelper
    {

        public static bool IsInterfaceType(Type type)
        {

            return GetInterfacesExcludeIocType(type).Length > 0;

        }


        public static bool IsClassType(Type type)
        {

            return GetInterfacesExcludeIocType(type).Length == 0;

        }


        public static Type[] GetInterfacesExcludeIocType(Type type)
        {

            return type.GetInterfaces().Where(m => m != typeof(ITransientDependency) && m != typeof(ISingletonDependency)).ToArray();

        }


    }
}
