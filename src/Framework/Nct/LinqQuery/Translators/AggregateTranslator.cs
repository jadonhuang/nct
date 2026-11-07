using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Nct.LinqQuery
{
    public class AggregateTranslator : BaseTranslator
    {
        public AggregateTranslator(SqlFormatter formatter) : base(formatter)
        {

        }

        public override void Translate(Expression expression)
        {
            MethodCallExpression methodCall = expression as MethodCallExpression;

            string name = methodCall.Method.Name;

            string columnName = null;

            if (methodCall.Arguments.Count == 2)
            {

                var member = ((StripQuotes(methodCall.Arguments[1]) as LambdaExpression).Body) as MemberExpression;
                columnName = SqlFormatter.FormatQualifiedColumnName(member.Expression.Type, member.Member.Name);
            }

            SqlFormatter.FormatAggregate(this, name, columnName);

        }
    }
}
