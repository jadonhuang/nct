
using Nct.Domain.Entities;
using Nct.Extentions;
using Nct.Session;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using static Nct.DataAccess.DynamicQuery.Constants;

namespace Nct.DataAccess.DynamicQuery
{
    public class SqlBuilderHelper
    {



        private static void CheckObjTypeIsValid(object obj)
        {
            if (obj is DynamicRow) throw new ArgumentException($"inserting {nameof(obj)} is not a surported type");

        }


        private static void BuildSqlAudit(DynamicSqlInfo sqlInfo, bool isInsert)
        {

            if (!sqlInfo.NeedAudit) return;

            var dic = new Dictionary<string, object>();
            var session = ISession.Session;
            if (isInsert)
            {
                dic.Add("Id", Guid.NewGuid().ToString());
                dic.Add("CreateTime", DateTime.Now);
                dic.Add("CreatorId", session.UserId);
                dic.Add("CreateUser", session.UserName);
            }
            else
            {
                dic.Add("UpdateTime", DateTime.Now);
                dic.Add("UpdateUser", session.UserName);
            }

            foreach (var kv in dic)
            {

                var field = kv.Key;
                if (!sqlInfo.FieldList.Any(m => m.IgnoreCaseEqual(field)))
                {
                    sqlInfo.FieldList.Add(field);

                }

                sqlInfo.Arguments.TryGetValue(field, out var value);

                if (value == null)
                {
                    sqlInfo.Arguments[field] = kv.Value;

                }


            }


        }

        public static void BuildSqlInfo(DynamicSqlInfo sqlInfo)
        {

            if (sqlInfo.SqlType == SqlKeyword.Insert)
            {

                BuildInsertSqlInfo(sqlInfo);
            }
            else if (sqlInfo.SqlType == SqlKeyword.Update)
            {

                BuildUpdateSqlInfo(sqlInfo);
            }

        }



        private static void BuildInsertSqlInfo(DynamicSqlInfo sqlInfo)
        {


            object obj = sqlInfo.Data;

            if (obj is IDictionary<string, object> dic)
            {

                sqlInfo.FieldList = [.. dic.Keys];
                sqlInfo.Arguments = dic;

            }
            else
            {

                CheckObjTypeIsValid(obj);
                EntityAuditingHelper.SetCreationAuditProperties(obj);
                Type type = obj.GetType();
                PropertyInfo[] props = type.GetProperties();

                for (int i = 0; i < props.Length; i++)
                {
                    var propName = props[i].Name;
                    sqlInfo.FieldList.Add(propName);
                    sqlInfo.Arguments[propName] = props[i].GetValue(obj, null);

                }

            }
            BuildSqlAudit(sqlInfo, true);
            var insertPart = $"({string.Join(",", sqlInfo.FieldList)}) VALUES ({string.Join(",", sqlInfo.FieldList.Select(m => "@" + m))})";
            sqlInfo.Fields = insertPart;
        }


        public static string GetArgName(DynamicSqlInfo sqlInfo)
        {

            return $"@p{sqlInfo.Arguments.Keys.Count}";
        }


        private static void BuildUpdateSqlInfo(DynamicSqlInfo sqlInfo)
        {
            MethodCallExpression node = sqlInfo.Data as MethodCallExpression;
            UnaryExpression unaryExp = (UnaryExpression)node.Arguments[1];
            var lambda = unaryExp.Operand as LambdaExpression;
            var sb = new StringBuilder();

            if (lambda.Body.NodeType == ExpressionType.Constant)
            {
                object obj = (lambda.Body as ConstantExpression).Value;

                if (obj is IDictionary<string, object> dic)
                {

                    foreach (var kv in dic)
                    {
                        var propName = kv.Key;
                        sqlInfo.FieldList.Add(propName);
                        sqlInfo.Arguments[propName] = kv.Value;


                    }

                }
                else
                {
                    CheckObjTypeIsValid(obj);
                    EntityAuditingHelper.SetUpdateAuditProperties(obj);
                    Type type = obj.GetType();
                    PropertyInfo[] props = type.GetProperties();
                    for (int i = 0; i < props.Length; i++)
                    {
                        var propName = props[i].Name;
                        sqlInfo.FieldList.Add(propName);
                        sqlInfo.Arguments[propName] = props[i].GetValue(obj, null);

                    }
                }

            }
            else if (lambda.Body.NodeType == ExpressionType.New)
            {

                var newExpression = (NewExpression)lambda.Body;
                var parts = new Stack<string>();
                var methodStack = new Stack<string>();

                int len = newExpression.Members.Count;
                for (int i = 0; i < len; i++)
                {
                    parts.Clear();
                    methodStack.Clear();
                    string propName = newExpression.Members[i].Name;

                    var arg = newExpression.Arguments[i];

                    if (arg.NodeType != ExpressionType.Constant)
                    {

                        VisitUpdateExpression(arg, parts, methodStack, sqlInfo);

                    }
                    else
                    {
                        var argName = GetArgName(sqlInfo);
                        parts.Push(argName);
                        sqlInfo.Arguments[argName] = (arg as ConstantExpression).Value;
                    }
                    sb.AppendFormat("{0}={1},", propName, parts.Pop());



                }

            }

            BuildSqlAudit(sqlInfo, false);
            foreach (var field in sqlInfo.FieldList)
            {
                sb.AppendFormat("{0} = @{0},", field);
            }

            var fields = sb.ToString().TrimEnd(',');
            sqlInfo.Fields = fields;

        }

        private static void VisitUpdateExpression(Expression expression, Stack<string> parts, Stack<string> methodStack, DynamicSqlInfo sqlInfo)
        {
            if (expression == null) return;
            switch (expression.NodeType)
            {
                case ExpressionType.Call:
                    MethodCallExpression method = expression as MethodCallExpression;
                    VisitUpdateExpression(method.Object, parts, methodStack, sqlInfo);
                    var methodName = method.Method.Name;
                    methodStack.Push(methodName);
                    string format = null;
                    if (methodName == LinqFunction.Replace)
                    {
                        format = $"{SqlKeyword.Replace}(#source#,{{1}},{{0}})";
                    }
                    else if (methodName == LinqFunction.GetItem)
                    {

                    }
                    else
                    {
                        throw new NotSupportedException($"{methodName} 不支持");
                    }
                    for (int i = 0; i < method.Arguments.Count; i++)
                    {
                        VisitUpdateExpression(method.Arguments[i], parts, methodStack, sqlInfo);

                    }

                    if (format != null)
                    {
                        var part0 = parts.Pop();
                        var part1 = parts.Pop();
                        format = string.Format(format, part0, part1);
                        if (parts.Count > 0)
                        {
                            format = format.Replace("#source#", parts.Pop());
                        }

                        parts.Push(format);
                    }



                    break;

                case ExpressionType.New:
                    var newExpression = (NewExpression)expression;
                    for (int i = newExpression.Arguments.Count - 1; i >= 0; i--)
                        VisitUpdateExpression(newExpression.Arguments[i], parts, methodStack, sqlInfo);

                    break;
                case ExpressionType.Lambda:
                    LambdaExpression lambda = expression as LambdaExpression;
                    VisitUpdateExpression(lambda.Body, parts, methodStack, sqlInfo);
                    break;
                case ExpressionType.Add:
                case ExpressionType.Subtract:
                case ExpressionType.Multiply:
                case ExpressionType.Divide:

                    BinaryExpression binary = expression as BinaryExpression;
                    string opr = string.Empty;
                    if (expression.NodeType == ExpressionType.Add) opr = SqlOperator.Add;
                    else if (expression.NodeType == ExpressionType.Subtract) opr = SqlOperator.Subtract;
                    else if (expression.NodeType == ExpressionType.Multiply) opr = SqlOperator.Multiply;
                    else if (expression.NodeType == ExpressionType.Divide) opr = SqlOperator.Divide;

                    string format2 = "{0} {1} {2}";
                    VisitUpdateExpression(binary.Left, parts, methodStack, sqlInfo);
                    VisitUpdateExpression(binary.Right, parts, methodStack, sqlInfo);

                    var left = parts.Pop();
                    var right = parts.Pop();
                    format2 = string.Format(format2, right, opr, left);
                    parts.Push(format2);

                    break;
                case ExpressionType.Constant:
                    ConstantExpression constant = expression as ConstantExpression;

                    string preMethod = null;

                    if (methodStack.Count > 0)
                    {
                        preMethod = methodStack.Pop();
                    }
                    if (preMethod == null || preMethod == LinqFunction.Replace)
                    {
                        var argName = $"@arg{sqlInfo.Arguments.Keys.Count}";
                        sqlInfo.Arguments[argName] = constant.Value;
                        parts.Push(argName);
                    }
                    else if (preMethod == LinqFunction.GetItem)
                    {
                        parts.Push(constant.Value.ToString());

                    }

                    break;
                case ExpressionType.MemberAccess:
                    //MemberExpression Member = expression as MemberExpression;
                    break;
                default:
                    break;
            }
        }

    }
}
