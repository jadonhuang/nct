using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;



namespace Nct.DataAccess.DynamicQuery.Extensions
{
    public static class DynamicQueryableExtensions
    {


        internal static IQueryable<DynamicRow> From<DynamicRow>(this IQueryable<DynamicRow> source, string table)
        {

            MethodInfo method = new Func<IQueryable<DynamicRow>, string, IQueryable<DynamicRow>>(From).Method;

            return source.Provider.CreateQuery<DynamicRow>(Expression.Call(null, method, new Expression[]
            {
                source.Expression,
               Expression.Constant(table)
            }

            ));


        }

        public static IQueryable<DynamicRow> WithAudit<DynamicRow>(this IQueryable<DynamicRow> source)
        {

            MethodInfo method = new Func<IQueryable<DynamicRow>, IQueryable<DynamicRow>>(WithAudit).Method;

            return source.Provider.CreateQuery<DynamicRow>(Expression.Call(null, method, new Expression[]
            {
                source.Expression
            }

            ));


        }

        public static int Update<TObj>(this IQueryable<DynamicRow> source, Expression<Func<DynamicRow, TObj>> selector)
        {

            MethodInfo method = new Func<IQueryable<DynamicRow>, Expression<Func<DynamicRow, TObj>>, int>(Update).Method;

            return source.Provider.Execute<int>(Expression.Call(null, method, new Expression[]
            {
                source.Expression,
               Expression.Quote(selector)
            }

            ));


        }


        public static Task<int> UpdateAsync<TObj>(this IQueryable<DynamicRow> source, Expression<Func<DynamicRow, TObj>> selector)
        {

            return Task.FromResult(Update(source, selector));


        }

        public static int Insert<TObj>(this IQueryable<DynamicRow> source, TObj obj)
        {


            MethodInfo method = new Func<IQueryable<DynamicRow>, TObj, int>(Insert).Method;

            return source.Provider.Execute<int>(Expression.Call(null, method, new Expression[]
            {
                source.Expression,
               Expression.Constant(obj)
            }

            ));


        }

        public static Task<int> InsertAsync<TObj>(this IQueryable<DynamicRow> source, TObj obj)
        {

            return Task.FromResult(Insert(source, obj));

        }





        public static IQueryable<DynamicRow> Fields<DynamicRow>(this IQueryable<DynamicRow> source, string fields)
        {

            MethodInfo method = new Func<IQueryable<DynamicRow>, string, IQueryable<DynamicRow>>(Fields).Method;

            return source.Provider.CreateQuery<DynamicRow>(Expression.Call(null, method, new Expression[]
            {
                source.Expression,
               Expression.Constant(fields)
            }

            ));


        }



        public static int DeleteRows<T>(IQueryable<T> source)
        {


            MethodInfo method = new Func<IQueryable<T>, int>(DeleteRows).Method;

            return source.Provider.Execute<int>(Expression.Call(null, method, new Expression[]
            {
                source.Expression
            }

            ));

        }



    }
}
