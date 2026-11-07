using AutoMapper;
using Nct.Ioc;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Nct.ObjectMapper
{
    public static partial class Extensions
    {

        private static IMapper GetMapper()
        {

            return IocManager.Instance.Resolve<IMapper>();
        
        }

        public static List<TResult> MapTo<TResult>(this IEnumerable self)
        {
            if (self == null) return null;
            return (List<TResult>)GetMapper().Map(self, self.GetType(), typeof(List<TResult>));
        }


        public static TResult MapTo<TResult>(this object self)
        {
            if (self == null) return default ;
            return (TResult)GetMapper().Map(self, self.GetType(), typeof(TResult));
        }


        public static void MapFrom<TSource, TResult>(this TResult result, TSource source)
        {
            GetMapper().Map(source, result);
        }



    }
}
