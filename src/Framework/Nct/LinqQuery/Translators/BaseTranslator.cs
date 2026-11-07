using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nct.LinqQuery
{
    public abstract class BaseTranslator : ExpressionVisitor
    {


        public BaseTranslator(SqlFormatter formatter)
        {
            SqlFormatter = formatter;
        }

        public SqlFormatter SqlFormatter { get; set; }

        public StringBuilder SqlPart { get; } = new StringBuilder();


        public void Write(string str)
        {
            SqlPart.Append(str);

        }



        public abstract void Translate(Expression expression);


        public Expression StripQuotes(Expression e)
        {

            while (e.NodeType == ExpressionType.Quote)
            {

                e = ((UnaryExpression)e).Operand;

            }

            return e;

        }


        public NewExpression FindNewExpress(Expression e)
        {


            var expression = e;

            NewExpression newExp = null;

            while (true)
            {
                if (expression is not MethodCallExpression callExp) break;

                if (callExp.Arguments[1].NodeType == ExpressionType.Quote)
                {
                    var exp = StripQuotes(callExp.Arguments[1]);
                    if (exp is LambdaExpression lambda && lambda.Body is NewExpression newExpression)
                    {
                        return newExpression;
                    }

                }

                expression = callExp.Arguments[0];

            }
            return newExp;

        }




    }
}
