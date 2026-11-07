using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Nct.DataAccess.DynamicQuery
{
    public class DynamicQueryable<T> : IOrderedQueryable<T>
    {

      

        public DynamicQueryable()
        {

            Provider = new DynamicQueryProvider();       
            Expression = Expression.Constant(this);
        }

        public DynamicQueryable(Expression expression)
        {
        
            Provider = new DynamicQueryProvider();
            Expression = expression;
        }
        public Type ElementType
        {
            get { return typeof(T); }
        }
        public Expression Expression { get; }
        public IQueryProvider Provider { get; }

        public IEnumerator GetEnumerator()
        {

            return (Provider.Execute(Expression) as IEnumerable).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var result = Provider.Execute<List<T>>(Expression);
            if (result == null)
                yield break;
            foreach (var item in result)
            {
                yield return item;
            }
        }
    }
}
