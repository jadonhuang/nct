using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Nct.LinqQuery
{
    public class UpdaterTranslator : BaseTranslator
    {
        public UpdaterTranslator(SqlFormatter formatter) : base(formatter)
        { }

        public override void Translate(Expression expression)
        {
            LambdaExpression lambda = StripQuotes(expression) as LambdaExpression;
            base.Visit(lambda.Body);

        }

        protected override Expression VisitBinary(BinaryExpression b)
        {

            Expression left = b.Left;
            Expression right = b.Right;
            this.Visit(left);
            switch (b.NodeType)
            {

                case ExpressionType.Add:
                case ExpressionType.AddAssign:
                    Write(" + ");
                    break;

                case ExpressionType.Subtract:
                case ExpressionType.SubtractAssign:
                    Write(" - ");
                    break;

                case ExpressionType.Multiply:
                case ExpressionType.MultiplyAssign:
                    Write(" * ");
                    break;

                case ExpressionType.Divide:
                case ExpressionType.DivideAssign:
                    Write(" / ");
                    break;
                default:
                    throw new NotSupportedException($"{b.NodeType} not supported");
                
            }

            this.Visit(right);
            return b;


        }


        protected override Expression VisitMember(MemberExpression node)
        {

            Write(SqlFormatter.FormatQualifiedColumnName(node.Expression.Type, node.Member.Name));
            return node;

        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            SqlFormatter.FormatMethod(this, node);
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {

            Write(SqlFormatter.AddParam(node.Value));
            Write(Environment.NewLine);
            return base.VisitConstant(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            return base.VisitNew(node);
        }

        protected override Expression VisitMemberInit(MemberInitExpression node)
        {
            var type = node.NewExpression.Type;
            foreach (var member in node.Bindings)
            {
                MemberAssignment assignment = member as MemberAssignment;
                if (SqlPart.Length > 0) Write(",");
                Write(SqlFormatter.FormatQualifiedColumnName(type, assignment.Member.Name));
                Write(" = ");
                Visit(assignment.Expression);

            }
            return node;
        }



    }
}
