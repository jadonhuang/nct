using Nct.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
namespace Nct.LinqQuery
{


    public static class QueryExtensions
    {

        private readonly static MethodInfo _updateObject = GetQueryExtensionMethod("Update", 1, mi =>
        {

            var ps = mi.GetParameters();

            return !ps[1].ParameterType.IsGenericType;

        });



        private readonly static MethodInfo _updateSelector = GetQueryExtensionMethod("Update", 1, mi =>
        {

            var ps = mi.GetParameters();

            return ps[1].ParameterType.IsGenericType;

        });


        private readonly static MethodInfo _delete = GetQueryExtensionMethod("Delete", 0);

        private readonly static MethodInfo _deleteObject = GetQueryExtensionMethod("Delete", 1);

        private readonly static MethodInfo _insert = GetQueryExtensionMethod("Insert", 1);

        private readonly static MethodInfo _firstOrDefault = GetQueryableMethod("FirstOrDefault", 0);

        private readonly static MethodInfo _count = GetQueryableMethod("Count");

        private readonly static MethodInfo _longCount = GetQueryableMethod("LongCount", 0);


        private static MethodInfo GetQueryableMethod(string name, int parameterCount = 0, Func<MethodInfo, bool> predicate = null)
        {
            return GetMethod(typeof(Queryable), name, parameterCount, predicate);
        }


        private static MethodInfo GetMethod(Type type, string name, int parameterCount = 0, Func<MethodInfo, bool> predicate = null)
        {

            return type.GetTypeInfo().GetDeclaredMethods(name).Single((MethodInfo mi) => mi.GetParameters().Length == parameterCount + 1 && (predicate == null || predicate(mi)));

        }

        private static MethodInfo GetQueryExtensionMethod(string name, int parameterCount = 0, Func<MethodInfo, bool> predicate = null)
        {
            return GetMethod(typeof(QueryExtensions), name, parameterCount, predicate);
        }


        private static MethodInfo GetMethodInfo<T1, T2>(Func<T1, T2> f, T1 unused1)
        {
            return f.Method;
        }



        private static TResult Execute<TSource, TResult>(MethodInfo operatorMethodInfo, IQueryable<TSource> source, Expression expression)
        {

            operatorMethodInfo = (operatorMethodInfo.GetGenericArguments().Length == 2) ? operatorMethodInfo.MakeGenericMethod(new Type[]
                    {
                        typeof(TSource),
                        typeof(TResult)
                    }) : operatorMethodInfo.MakeGenericMethod(new Type[]
                    {
                        typeof(TSource)
                    });
            return source.Provider.Execute<TResult>(Expression.Call(null, operatorMethodInfo, new Expression[]
             {
                source.Expression,
                expression
             }));

        }


        private static TResult Execute<TSource, TResult>(MethodInfo operatorMethodInfo, IQueryable<TSource> source)
        {

            if (operatorMethodInfo.IsGenericMethod)
            {
                operatorMethodInfo = operatorMethodInfo.MakeGenericMethod(new Type[]
                {
                     typeof(TSource)
                });
            }
            return source.Provider.Execute<TResult>(Expression.Call(null, operatorMethodInfo, new Expression[]
            {
                source.Expression
            }));

        }





        private static Task<TResult> ExecuteAsync<TSource, TResult>(MethodInfo operatorMethodInfo, IQueryable<TSource> source, Expression expression)
        {

            var asyncProvider = source.Provider as IAsyncQueryProvider;

            operatorMethodInfo = (operatorMethodInfo.GetGenericArguments().Length == 2) ? operatorMethodInfo.MakeGenericMethod(new Type[]
                    {
                        typeof(TSource),
                        typeof(TResult)
                    }) : operatorMethodInfo.MakeGenericMethod(new Type[]
                    {
                        typeof(TSource)
                    });

            return asyncProvider.ExecuteAsync<TResult>(Expression.Call(null, operatorMethodInfo, new Expression[]
             {
                source.Expression,
                expression
             }));

        }


        private static Task<TResult> ExecuteAsync<TSource, TResult>(MethodInfo operatorMethodInfo, IQueryable<TSource> source)
        {

            var asyncProvider = source.Provider as IAsyncQueryProvider;

            if (operatorMethodInfo.IsGenericMethod)
            {
                operatorMethodInfo = operatorMethodInfo.MakeGenericMethod(new Type[]
                {
                     typeof(TSource)
                });
            }
            return asyncProvider.ExecuteAsync<TResult>(Expression.Call(null, operatorMethodInfo, new Expression[]
            {
                    source.Expression
            }));

        }


        private static Task<TResult> ExecuteAsync<TSource, TResult>(IQueryable<TSource> source)
        {
            var asyncProvider = source.Provider as IAsyncQueryProvider;
            return asyncProvider.ExecuteAsync<TResult>(source.Expression);

        }



        public static int Update<T>(this IQueryable<T> source, Expression<Func<T, T>> selector)
        {

            return Execute<T, int>(_updateSelector, source, Expression.Quote(selector));


        }


        public static Task<int> UpdateAsync<T>(this IQueryable<T> source, Expression<Func<T, T>> selector)
        {

            return ExecuteAsync<T, int>(_updateSelector, source, Expression.Quote(selector));

        }



        public static int Update<T>(this IQueryable<T> source, T obj)
        {
            EntityAuditingHelper.SetUpdateAuditProperties(obj);
            return Execute<T, int>(_updateObject, source, Expression.Constant(obj));

        }


        public static Task<int> UpdateAsync<T>(this IQueryable<T> source, T obj)
        {
            EntityAuditingHelper.SetUpdateAuditProperties(obj);
            return ExecuteAsync<T, int>(_updateObject, source, Expression.Constant(obj));

        }

        private static Expression<Func<T, T>> BuildDeleteSelector<T>(IQueryable<T> source)
        {

            var type = typeof(T);

            var newExp = Expression.New(type);

            var bindExp = Expression.Bind(type.GetMember("IsDeleted")[0], Expression.Constant(true));

            var memberExp = Expression.MemberInit(newExp, bindExp);

            var pramExp = Expression.Parameter(type);

            return Expression.Lambda<Func<T, T>>(memberExp, pramExp);


        }


        public static int Delete<T>(this IQueryable<T> source)
        {

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
            {
                var selector = BuildDeleteSelector(source);
                return Update(source, selector);
            }

            return Execute<T, int>(_delete, source);

        }

        public static Task<int> DeleteAsync<T>(this IQueryable<T> source)
        {

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(T)))
            {

                var selector = BuildDeleteSelector(source);
                return UpdateAsync(source, selector);
            }

            return ExecuteAsync<T, int>(_delete, source);

        }


        public static IQueryable<T> WithNoLock<T>(this IQueryable<T> source)
        {


            return source.Provider.CreateQuery<T>(Expression.Call(null, GetMethodInfo(WithNoLock, source), new Expression[1]
            {
                    source.Expression,
            }));

        }


        public static int Delete<T>(this IQueryable<T> source, T obj)
        {
            if (obj is ISoftDelete deleteObj)
            {
                deleteObj.IsDeleted = true;
                return Update(source, obj);
            }
            return Execute<T, int>(_deleteObject, source, Expression.Constant(obj));
        }

        public static Task<int> DeleteAsync<T>(this IQueryable<T> source, T obj)
        {
            if (obj is ISoftDelete deleteObj)
            {
                deleteObj.IsDeleted = true;
                return UpdateAsync(source, obj);
            }

            return ExecuteAsync<T, int>(_deleteObject, source, Expression.Constant(obj));

        }


        public static int Insert<T>(this IQueryable<T> source, T obj)
        {
            EntityAuditingHelper.SetCreationAuditProperties(obj);
            return Execute<T, int>(_insert, source, Expression.Constant(obj));

        }


        public static Task<int> InsertAsync<T>(this IQueryable<T> source, T obj)
        {
            EntityAuditingHelper.SetCreationAuditProperties(obj);
            return ExecuteAsync<T, int>(_insert, source, Expression.Constant(obj));

        }


        public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source)
        {

            return ExecuteAsync<T, T>(_firstOrDefault, source);

        }


        public static Task<int> CountAsync<T>(this IQueryable<T> source)
        {

            return ExecuteAsync<T, int>(_count, source);
        }




        public static Task<long> LongCountAsync<T>(this IQueryable<T> source)
        {

            return ExecuteAsync<T, long>(_longCount, source);

        }


        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> source)
        {

            return (await ExecuteAsync<T, IEnumerable<object>>(source)).Select(m => (T)m).ToList();


        }








    }
}
