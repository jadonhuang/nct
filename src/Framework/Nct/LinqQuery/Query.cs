using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nct.LinqQuery
{
    public class Query<T> : IQueryable<T>, IQueryable, IEnumerable<T>, IEnumerable, IOrderedQueryable<T>, IOrderedQueryable
    {

        public Query(QueryProvider provider)
        {

            if (provider == null)
            {

                throw new ArgumentNullException(nameof(provider));

            }

            Provider = provider;
            Expression = Expression.Constant(this);

        }



        public Query(QueryProvider provider, Expression expression)
        {

            if (provider == null)
            {

                throw new ArgumentNullException(nameof(provider));

            }

            if (expression == null)
            {

                throw new ArgumentNullException(nameof(expression));

            }

            if (!typeof(IQueryable<T>).IsAssignableFrom(expression.Type))
            {

                throw new ArgumentOutOfRangeException(nameof(expression));

            }

            Provider = provider;
            Expression = expression;

        }

        public Type ElementType => typeof(T);

        public Expression Expression { get; }

        public IQueryProvider Provider { get; }

        public IEnumerator<T> GetEnumerator()
        {

            IEnumerable<object> list = (IEnumerable<object>)Provider.Execute(Expression);

            foreach (var item in list)
            {

                yield return (T)item;
            }



        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)Provider.Execute(Expression)).GetEnumerator();
        }
    }
}
