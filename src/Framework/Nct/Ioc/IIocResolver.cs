using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Ioc
{
    public interface IIocResolver
    {
        bool IsRegistered(Type type);


        bool IsRegistered<TType>();


        T Resolve<T>();


        T ResolveOrNull<T>();


        T Resolve<T>(Type type);


        T Resolve<T>(IDictionary<string, object> args);


        object Resolve(Type type);


        void Release(object obj);



    }
}
