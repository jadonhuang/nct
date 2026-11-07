using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.Core
{
    public static class DbInterception
    {
        private static readonly List<IDbCommandInterceptor> _interceptors = [];

        public static void Add(IDbCommandInterceptor interceptor)
        {

            _interceptors.Add(interceptor);


        }
        public static void Remove(IDbCommandInterceptor interceptor)
        {

            _interceptors.Remove(interceptor);

        }

        public static IDbCommandInterceptor[] GetInterceptors()
        {
            return [.. _interceptors];
        }


        public static void Executing(DbCommandInfo commandInfo)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.Executing(commandInfo);

            }
        }


        public static void Executed(DbCommandInfo commandInfo)
        {
            foreach (var interceptor in _interceptors)
            {
                interceptor.Executed(commandInfo);

            }
        }



    }
}
