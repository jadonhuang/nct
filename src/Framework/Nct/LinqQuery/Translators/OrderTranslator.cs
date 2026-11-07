using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using Nct.Extentions;

namespace Nct.LinqQuery
{
    public class OrderTranslator : BaseTranslator
    {

        private string orderBy = null;
        private Expression source = null;
        public OrderTranslator(SqlFormatter formatter) : base(formatter)
        {

        }

        public override void Translate(Expression expression)
        {

            OrderExpression orderExpression = expression as OrderExpression;
            source = orderExpression.Source;

            if (SqlPart.Length > 0)
            {
                Write(",");
            }
            else
            {
                Write("ORDER BY ");
            }

            orderBy = orderExpression.OrderType == OrderType.Desc ? " DESC " : "";

            LambdaExpression lambda = StripQuotes(orderExpression.OrderBy) as LambdaExpression;

            if (lambda.Body.NodeType == ExpressionType.MemberAccess)
            {
                Visit(lambda.Body);

            }

            else if (lambda.Body.NodeType == ExpressionType.New)
            {

                NewExpression newExpression = lambda.Body as NewExpression;

                int index = 0;

                foreach (var exp in newExpression.Arguments)
                {
                    if (index > 0) Write(",");
                    MemberExpression member = exp as MemberExpression;

                    Write($"{SqlFormatter.FormatQualifiedColumnName(member.Expression.Type, member.Member.Name)}{orderBy}");

                }

            }
            else
            {
                var entity = EntityCache.GetOrAdd(lambda.ReturnType);

                var cols = entity.Fields.Select(m => $"{SqlFormatter.FormatQualifiedColumnName(entity.EntityType, m.ColumnName)}{orderBy}");

                string orderStr = string.Join(",", cols);

                Write(orderStr);
            }


        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression != null && (node.Expression.NodeType == ExpressionType.Parameter || node.Expression.NodeType == ExpressionType.MemberAccess))
            {
                var type = node.Expression.Type;
                if (type.IsAnonymousType())
                {

                    var newExp = FindNewExpress(source);
                    var memberName = node.Member.Name;
                    var memberExp = newExp.Arguments.First(m => ((MemberExpression)m).Member.Name == memberName);
                    type = ((MemberExpression)memberExp).Expression.Type;

                }
              
               Write(SqlFormatter.FormatQualifiedColumnName(type, node.Member.Name));

            }
            else
            {
                Write(SqlFormatter.FormatQualifiedColumnName(node.Member.DeclaringType, node.Member.Name));
            }
            Write(orderBy);
            return node;

        }








    }
}
