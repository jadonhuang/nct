using Nct.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nct.LinqQuery
{
    public abstract class QueryProvider : IQueryProvider, IAsyncQueryProvider
    {


        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
        {

            return new Query<TElement>(this, expression);

        }



        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {

            Type elementType = ReflectionHelper.GetElementType(expression.Type);
            try
            {
                return (IQueryable)Activator.CreateInstance(typeof(Query<>).MakeGenericType(elementType), new object[] { this, expression });
            }
            catch (TargetInvocationException ex)
            {
                throw ex.InnerException;
            }

        }



        TResult IQueryProvider.Execute<TResult>(Expression expression)
        {

            return (TResult)Execute(expression);

        }



        object IQueryProvider.Execute(Expression expression)
        {

            return Execute(expression);

        }


        public abstract object Execute(Expression expression);

        public abstract Task<TResult> ExecuteAsync<TResult>(Expression expression);



    }


}
