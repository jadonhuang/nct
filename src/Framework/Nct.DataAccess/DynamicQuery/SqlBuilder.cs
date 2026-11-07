using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using static Nct.DataAccess.DynamicQuery.Constants;

namespace Nct.DataAccess.DynamicQuery
{
    public class SqlBuilder : ExpressionVisitor
    {



        private readonly Stack<string> _cdtParts = new();

        private readonly Stack<string> _methodStack = new();

        private bool _isNot = false;

        public DynamicSqlInfo SqlInfo { get; private set; } = new DynamicSqlInfo();


        public void Build()
        {
            if (_cdtParts.Count > 0)
            {

                SqlInfo.Condition = string.Join($" {SqlOperator.AndAlso} ", _cdtParts.ToArray().Reverse());
                _cdtParts.Clear();
            }

            SqlBuilderHelper.BuildSqlInfo(SqlInfo);

        }

        protected override Expression VisitBinary(BinaryExpression node)
        {


            if (node == null) return node;

            string opr;
            switch (node.NodeType)
            {
                case ExpressionType.Equal:
                    opr = SqlOperator.Equal;
                    break;
                case ExpressionType.NotEqual:
                    opr = SqlOperator.NotEqual;
                    break;
                case ExpressionType.GreaterThan:
                    opr = SqlOperator.GreaterThan;
                    break;
                case ExpressionType.GreaterThanOrEqual:
                    opr = SqlOperator.GreaterThanOrEqual;
                    break;
                case ExpressionType.LessThan:
                    opr = SqlOperator.LessThan;
                    break;
                case ExpressionType.LessThanOrEqual:
                    opr = SqlOperator.LessThanOrEqual;
                    break;
                case ExpressionType.AndAlso:
                    opr = SqlOperator.AndAlso;
                    break;
                case ExpressionType.OrElse:
                    opr = SqlOperator.OrElse;
                    break;

                case ExpressionType.Add:
                    opr = SqlOperator.Add;
                    break;
                case ExpressionType.Subtract:
                    opr = SqlOperator.Subtract;
                    break;
                case ExpressionType.Multiply:
                    opr = SqlOperator.Multiply;
                    break;
                case ExpressionType.Divide:
                    opr = SqlOperator.Divide;
                    break;

                default:
                    throw new NotSupportedException(node.NodeType + " 不支持");
            }


            var exp = base.VisitBinary(node);

            string right = _cdtParts.Pop();
            string left = _cdtParts.Pop();
            string condition = null;

            if (node.Right != null && node.Right.ToString() == "False" && left.Contains($" {SqlOperator.Like} "))
            {

                left = left.Replace(SqlOperator.Like, SqlOperator.NotLike);
                condition = string.Format("{0}", left);
            }
            else
            {
                if (SqlInfo.Arguments.Keys.Contains(right) && SqlInfo.Arguments[right] == null)
                {
                    var nullExp = opr == SqlOperator.NotEqual ? SqlOperator.IsNotNull : SqlOperator.IsNull;
                    condition = string.Format("({0} {1})", left, nullExp);
                }
                else
                {

                    condition = string.Format("({0} {1} {2})", left, opr, right);
                }

            }


            _cdtParts.Push(condition);

            return exp;

        }


        protected override Expression VisitNew(NewExpression node)
        {
            return base.VisitNew(node);
        }




        protected override Expression VisitConstant(ConstantExpression node)
        {

            var queryable = node.Value as DynamicQueryable<DynamicRow>;
            if (queryable != null)
            {
                return node;
            }

            string preMethod = null;

            if (_methodStack.Count > 0)
            {
                preMethod = _methodStack.Peek();
            }


            if (preMethod == LinqFunction.GetItem)
            {
                _methodStack.Pop();
                _cdtParts.Push(node.Value.ToString());
            }

            else
            {
                var argName = SqlBuilderHelper.GetArgName(SqlInfo);
                SqlInfo.Arguments[argName] = node.Value;
                _cdtParts.Push(argName);

            }

            return base.VisitConstant(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            return base.VisitMember(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not)
            {
                _isNot = true;
            }
            return base.VisitUnary(node);
        }


        private object GetConstantValue(MethodCallExpression expression)
        {

            return (expression.Arguments[1] as ConstantExpression).Value;

        }

        private void VisitArgument(MethodCallExpression expression)
        {
            this.Visit(expression.Arguments[0]);

        }


        protected override Expression VisitMethodCall(MethodCallExpression node)
        {


            string format = null;
            var methodName = node.Method.Name;

            switch (methodName)
            {
                case LinqFunction.Take:
                    if (!SqlInfo.TopNum.HasValue)
                    {
                        SqlInfo.TopNum = (int)GetConstantValue(node);
                    }
                    VisitArgument(node);
                    return node;
                case LinqFunction.Skip:
                    SqlInfo.SkipNum = (int)GetConstantValue(node);
                    VisitArgument(node);
                    return node;

                case LinqFunction.WithAudit:
                    SqlInfo.NeedAudit = true;
                    break;
                case LinqFunction.From:
                    SqlInfo.TableName = (string)GetConstantValue(node);
                    return node;
                case LinqFunction.Fields:
                    if (SqlInfo.Fields == null)
                        SqlInfo.Fields = (string)GetConstantValue(node);
                    VisitArgument(node);
                    return node;

                case LinqFunction.FirstOrDefault:
                    SqlInfo.TopNum = 1;
                    SqlInfo.SkipNum = null;
                    break;
                case LinqFunction.Delete:
                case LinqFunction.DeleteRows:
                    SqlInfo.SqlType = SqlKeyword.Delete;
                    VisitArgument(node);
                    return node;
                case LinqFunction.ToList:
                    break;

                case LinqFunction.Max:
                case LinqFunction.Min:
                case LinqFunction.Average:
                case LinqFunction.Sum:
                    SqlInfo.SkipNum = null;
                    SqlInfo.TopNum = null;
                    SqlInfo.OrderBy = null;
                    UnaryExpression unaryExp2 = (UnaryExpression)node.Arguments[1];
                    var lambda2 = unaryExp2.Operand as LambdaExpression;
                    var field = ((ConstantExpression)((MethodCallExpression)((UnaryExpression)lambda2.Body).Operand).Arguments[0]).Value.ToString();
                    var aggMame = methodName == LinqFunction.Average ? SqlKeyword.Avarage : methodName.ToUpper();
                    SqlInfo.Fields = $"{aggMame}({field}) {field}";
                    VisitArgument(node);
                    return node;

                case LinqFunction.Count:
                    SqlInfo.SkipNum = null;
                    SqlInfo.TopNum = null;
                    SqlInfo.OrderBy = null;
                    SqlInfo.Fields = $"{SqlKeyword.Count}(1) num";
                    break;

                case LinqFunction.Contains:
                    format = $"({{0}} {(_isNot ? SqlOperator.NotLike : SqlOperator.Like)} '%'+{{1}}+'%')";
                    _isNot = false;
                    break;

                case LinqFunction.StartsWith:
                    format = $"({{0}} {(_isNot ? SqlOperator.NotLike : SqlOperator.Like)} {{1}}+'%')";
                    _isNot = false;
                    break;

                case LinqFunction.EndsWith:

                    format = $"({{0}} {(_isNot ? SqlOperator.NotLike : SqlOperator.Like)} '%'+{{1}})";
                    _isNot = false;
                    break;


                case LinqFunction.OrderBy:
                case LinqFunction.OrderByDesc:
                    UnaryExpression unaryExp = (UnaryExpression)node.Arguments[1];
                    var lambda = unaryExp.Operand as LambdaExpression;
                    if (lambda.Body.NodeType != ExpressionType.Constant)
                    {
                        throw new NotSupportedException("排序只支持字符串");

                    }

                    var sort = (lambda.Body as ConstantExpression).Value.ToString();
                    if (methodName == LinqFunction.OrderByDesc) sort += $" {SqlKeyword.Desc}";

                    if (string.IsNullOrEmpty(SqlInfo.OrderBy))
                        SqlInfo.OrderBy = sort;
                    else SqlInfo.OrderBy += $",{sort}";
                    VisitArgument(node);
                    return node;
                case LinqFunction.Where:
                case LinqFunction.GetItem:
                    _methodStack.Push(methodName);
                    break;
                case LinqFunction.Insert:
                    SqlInfo.Data= GetConstantValue(node);
                    SqlInfo.SqlType = SqlKeyword.Insert;
                    VisitArgument(node);
                    return node;
                case LinqFunction.Update:
                    SqlInfo.SqlType = SqlKeyword.Update;
                    VisitArgument(node);
                    SqlInfo.Data = node;
                    return node;
                default:
                    throw new NotSupportedException($"{methodName} 方法不支持");

            }

            var exp = base.VisitMethodCall(node);

            if (format != null)
            {
                var rigth = _cdtParts.Pop();
                var left = _cdtParts.Pop();

                if ((format.Contains(SqlOperator.NotLike) || format.Contains(SqlOperator.Like)) && left[0] == '@')
                {

                    var value = SqlInfo.Arguments[left];
                    if ((value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(List<>)) || value is Array)
                    {
                        var op = format.Contains(SqlOperator.NotLike) ? SqlOperator.NotIn : SqlOperator.In;
                        format = $"({rigth} {op} {left})";
                    }



                }


                format = string.Format(format, left, rigth);
                _cdtParts.Push(format);
            }

            return exp;
        }

        protected override Expression VisitBlock(BlockExpression node)
        {
            return base.VisitBlock(node);
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return base.VisitLambda(node);
        }
    }
}
