using Nct.Application;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Nct.Extentions
{
    public static class QueryableExtentions
    {

        public static IQueryable<T> WhereIf<T>(this IQueryable<T> queryable, Expression<Func<T, bool>> predicate, bool condition = true)
        {
            return condition ? queryable.Where(predicate) : queryable;

        }


        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, IPageQuery pageQuery)
        {
        
            var skip = (pageQuery.PageIndex - 1) * pageQuery.PageSize;
            return query.Skip(skip).Take(pageQuery.PageSize);

        }




    }
}
