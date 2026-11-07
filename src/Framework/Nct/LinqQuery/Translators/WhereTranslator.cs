using Nct.Extentions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Nct.LinqQuery
{


    public class WhereTranslator : BaseTranslator
    {


        private bool isMemberAccessExp = false;
        private Expression source;

        public WhereTranslator(SqlFormatter formatter) : base(formatter)
        { }

        public override void Translate(Expression expression)
        {

            if (expression is WhereExpression whereExpression)
            {
                source = whereExpression.Source;
                expression = whereExpression.Where;
            }

            if (SqlPart.Length > 0)
                Write(" AND ");
            else
                Write("WHERE ");

            if (expression.NodeType == ExpressionType.Quote)
            {
                LambdaExpression lambda = StripQuotes(expression) as LambdaExpression;
                isMemberAccessExp = lambda.Body.NodeType == ExpressionType.MemberAccess;
                base.Visit(lambda.Body);

            }
            else if (expression.NodeType == ExpressionType.Call)
            {
                var methodExp = expression as MethodCallExpression;

                for (int i = 1; i < methodExp.Arguments.Count; i++)
                {
                    LambdaExpression lambda = StripQuotes(methodExp.Arguments[i]) as LambdaExpression;
                    isMemberAccessExp = lambda.Body.NodeType == ExpressionType.MemberAccess;
                    base.Visit(lambda.Body);

                }

            }



        }

        protected virtual bool IsBoolean(Type type)
        {
            return type == typeof(bool) || type == typeof(bool?);
        }


        private bool IsPredicate(Expression expr)
        {
            switch (expr.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return IsBoolean(((BinaryExpression)expr).Type);
                case ExpressionType.Not:
                    return IsBoolean(((UnaryExpression)expr).Type);
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                    return true;
                case ExpressionType.Call:
                    return IsBoolean(((MethodCallExpression)expr).Type);
                default:
                    return false;
            }
        }

        private Expression VisitPredicate(Expression expr)
        {
            this.Visit(expr);
            if (!IsPredicate(expr))
            {
                this.Write(" = 1");
            }
            return expr;
        }

        private string GetOperator(BinaryExpression b)
        {
            switch (b.NodeType)
            {
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    return (IsBoolean(b.Left.Type)) ? "AND" : "&";
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return (IsBoolean(b.Left.Type) ? "OR" : "|");
                case ExpressionType.Equal:
                    return "=";
                case ExpressionType.NotEqual:
                    return "<>";
                case ExpressionType.LessThan:
                    return "<";
                case ExpressionType.LessThanOrEqual:
                    return "<=";
                case ExpressionType.GreaterThan:
                    return ">";
                case ExpressionType.GreaterThanOrEqual:
                    return ">=";
                case ExpressionType.Add:
                case ExpressionType.AddChecked:
                    return "+";
                case ExpressionType.Subtract:
                case ExpressionType.SubtractChecked:
                    return "-";
                case ExpressionType.Multiply:
                case ExpressionType.MultiplyChecked:
                    return "*";
                case ExpressionType.Divide:
                    return "/";
                case ExpressionType.Modulo:
                    return "%";
                default:
                    return "";
            }
        }



        protected override Expression VisitMethodCall(MethodCallExpression node)
        {

            if (node.Method.Name == "Equals")
            {
                if (node.Object.NodeType == ExpressionType.MemberAccess)
                {
                    MemberExpression member = node.Object as MemberExpression;

                    Type mtype = null;

                    if (member.Expression.NodeType == ExpressionType.Convert)
                    {
                        var ctExp = member.Expression as UnaryExpression;
                        mtype = ctExp.Operand.Type;

                    }
                    else
                    {
                        mtype = member.Expression.Type;
                    }

                    Write(SqlFormatter.FormatQualifiedColumnName(mtype, member.Member.Name));

                }
                else
                {

                    SqlFormatter.FormatMethod(this, node.Object as MethodCallExpression);
                }

                Write(" = ");
                Visit(node.Arguments[0]);

            }
            else
            {

                SqlFormatter.FormatMethod(this, node);
            }
            return node;
        }


        protected override Expression VisitBinary(BinaryExpression b)
        {


            string op = this.GetOperator(b);

            Write("(");

            Expression left = b.Left;
            Expression right = b.Right;

            switch (b.NodeType)
            {

                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    if (this.IsBoolean(left.Type))
                    {
                        this.VisitPredicate(left);
                        this.Write($" {op} ");
                        this.VisitPredicate(right);

                    }
                    else
                    {
                        this.Visit(left);
                        this.Write($" {op} ");
                        this.Visit(right);

                    }
                    break;
                case ExpressionType.Equal:
                    if (right.NodeType == ExpressionType.Constant)
                    {
                        ConstantExpression ce = (ConstantExpression)right;
                        if (ce.Value == null)
                        {
                            this.Visit(left);
                            this.Write(" IS NULL)");
                            return b;
                        }

                    }
                    else if (left.NodeType == ExpressionType.Constant)
                    {
                        ConstantExpression ce = (ConstantExpression)left;
                        if (ce.Value == null)
                        {
                            this.Visit(right);
                            this.Write(" IS NULL)");
                            return b;
                        }

                    }

                    this.Visit(left);
                    Write($" {op} ");
                    this.Visit(right);

                    break;

                case ExpressionType.NotEqual:
                    if (right.NodeType == ExpressionType.Constant)
                    {
                        ConstantExpression ce = (ConstantExpression)right;
                        if (ce.Value == null)
                        {
                            this.Visit(left);
                            this.Write(" IS NOT NULL)");
                            return b;

                        }

                    }
                    else if (left.NodeType == ExpressionType.Constant)
                    {
                        ConstantExpression ce = (ConstantExpression)left;
                        if (ce.Value == null)
                        {
                            this.Visit(right);
                            this.Write(" IS NOT NULL)");
                            return b;
                        }

                    }


                    this.Visit(left);
                    Write($" {op} ");
                    this.Visit(right);

                    break;

                case ExpressionType.LessThan:
                case ExpressionType.LessThanOrEqual:
                case ExpressionType.GreaterThan:
                case ExpressionType.GreaterThanOrEqual:
                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:
                    this.Visit(left);
                    Write($" {op} ");
                    this.Visit(right);
                    break;
                default:
                    throw new NotSupportedException(string.Format("The binary operator '{0}' is not supported", b.NodeType));

            }

            Write(")");
            return b;

        }




        protected override Expression VisitUnary(UnaryExpression u)
        {

            switch (u.NodeType)
            {

                case ExpressionType.Not:

                    Write(" NOT (");
                    VisitPredicate(u.Operand);
                    Write(")");
                    break;

                case ExpressionType.Convert:   //such as  m.id==int?            
                    base.Visit(u.Operand);
                    break;

                default:
                    break;


            }

            return u;

        }

        protected override Expression VisitMember(MemberExpression node)
        {

            if (node.Expression != null)
            {

                if (node.Expression.NodeType == ExpressionType.Parameter)
                {

                    var type = node.Expression.Type;
                    if (type.IsAnonymousType())
                    {

                        var newExp = FindNewExpress(source);
                        var memberName = node.Member.Name;
                        var memberExp = newExp.Arguments.First(m => ((MemberExpression)m).Member.Name == memberName);
                        type = ((MemberExpression)memberExp).Expression.Type;

                    }

                    Write($"{SqlFormatter.FormatQualifiedColumnName(type, node.Member.Name)}");

                    if (isMemberAccessExp)
                    {
                        Write(" =1");
                    }
                    return node;
                }

                if (node.Expression.NodeType == ExpressionType.MemberAccess)
                {

                    var prop = node.Member as PropertyInfo;
                    if (prop != null)
                    {
                        if (prop.PropertyType.IsClass)
                        {
                            Write($"{SqlFormatter.FormatQualifiedColumnName(node.Expression.Type, node.Member.Name)}");
                        }
                        else
                        {
                            var declareType = prop.DeclaringType;
                            if (declareType == typeof(string) || declareType == typeof(DateTime) || declareType == typeof(DateTimeOffset))
                            {
                                SqlFormatter.FormatMember(this, node);
                            }
                            else
                            {
                                Write($"{SqlFormatter.FormatQualifiedColumnName(declareType, prop.Name)}");

                            }

                        }

                    }


                }

            }

            return node;

        }




        protected override Expression VisitConstant(ConstantExpression node)
        {

            IQueryable q = node.Value as IQueryable;

            if (q == null)
            {

                var value = node.Value;

                if (node.Type.IsGenericType || node.Type.IsArray)
                {
                    if (value is not IEnumerable values)
                    {
                        Write(SqlFormatter.AddParam(value));
                    }
                    else
                    {

                        var ps = new List<string>();
                        foreach (var v in values)
                        {
                            var pName = SqlFormatter.AddParam(v);
                            ps.Add(pName);
                        }

                        Write(string.Join(",", ps));
                    }

                }
                else
                {
                    Write(SqlFormatter.AddParam(value));
                }




            }

            return node;

        }




    }
}
