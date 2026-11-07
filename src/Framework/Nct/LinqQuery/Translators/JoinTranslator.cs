using Nct.Extentions;
using Nct.Helpers;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Nct.LinqQuery
{
   public class JoinTranslator: BaseTranslator
    {
        public JoinTranslator(SqlFormatter formatter) : base(formatter)
        {

        }

        public override void Translate(Expression expression)
        {
           

            MethodCallExpression lambda = expression as MethodCallExpression;

          
            string joinType = "JOIN";  //very simple handle

            if (lambda.Method.Name == "GroupJoin")
            {
                joinType = "LEFT JOIN";
            }

            Type leftType = ReflectionHelper.GetElementType(lambda.Arguments[0].Type);

            Type rightType = ReflectionHelper.GetElementType(lambda.Arguments[1].Type);

            var leftTable = "";

            if (!leftType.IsAnonymousType())
            {
                leftTable = SqlFormatter.FormatTableNameAsAlias(leftType);

            }

            var rightTable = SqlFormatter.FormatTableNameAsAlias(rightType);

            var leftMember = ((StripQuotes(lambda.Arguments[2]) as LambdaExpression).Body) as MemberExpression;

            var leftField = SqlFormatter.FormatQualifiedColumnName(leftMember.Expression.Type, leftMember.Member.Name);

            var rightMember = ((StripQuotes(lambda.Arguments[3]) as LambdaExpression).Body) as MemberExpression;

            var rightField = SqlFormatter.FormatQualifiedColumnName(rightMember.Expression.Type, rightMember.Member.Name);


            Write($"{leftTable} {joinType} {rightTable} ON {leftField} = {rightField}");


        }
    }
}
