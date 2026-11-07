using Nct.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nct.LinqQuery
{
    public class GroupByTranslator:BaseTranslator
    {


        public GroupByTranslator(SqlFormatter formatter) : base(formatter)
        {

        }

        public override void Translate(Expression expression)
        {
           
         

            LambdaExpression lambda = StripQuotes(expression) as LambdaExpression;

            if (lambda.Body.NodeType == ExpressionType.Constant) return;


            if (SqlPart.Length > 0)
            {
                Write(",");
            }
            else
            {
                Write("GROUP BY ");
            }



            if (lambda.Body.NodeType == ExpressionType.MemberAccess|| lambda.Body.NodeType == ExpressionType.New)
            {
                Visit(lambda.Body);
               
            }
            else
            {
                var entity = EntityCache.GetOrAdd(lambda.ReturnType);

               var cols = entity.Fields.Select(m => $"{SqlFormatter.FormatQualifiedColumnName(entity.EntityType,m.ColumnName)}");
                string orderStr = string.Join(",", cols);
                Write(orderStr);
            }


        }

        protected override Expression VisitMember(MemberExpression m)
        {
            Write($"{ SqlFormatter.FormatQualifiedColumnName(m.Expression.Type, m.Member.Name)}");
            return m;

        }



        protected override Expression VisitNew(NewExpression node)
        {
         
            int index = 0;
            foreach (var exp in node.Arguments)
            {

                if (index > 0) Write(",");

                MemberExpression member = exp as MemberExpression;

                Write(SqlFormatter.FormatQualifiedColumnName(member.Expression.Type, member.Member.Name));

                index++;

            }

            return node;

        }


    }
}
