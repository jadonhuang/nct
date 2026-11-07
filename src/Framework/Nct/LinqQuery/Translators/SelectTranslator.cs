using Nct.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace Nct.LinqQuery
{
    public class SelectTranslator : BaseTranslator
    {
        public SelectTranslator(SqlFormatter formatter) : base(formatter)
        { }


        private List<GroupInfo> groupCols;

        public override void Translate(Expression expression)
        {

            var selectExp = expression as SelectExpression;

            ResolvingGroup(selectExp.GroupBy);

            expression = selectExp.Select;

            LambdaExpression lambda = StripQuotes(expression) as LambdaExpression;

            if (lambda.Body.NodeType == ExpressionType.New || lambda.Body.NodeType == ExpressionType.MemberInit || lambda.Body.NodeType == ExpressionType.MemberAccess)
            {

                bool isMemberClass = false;
                MemberExpression memberExp = lambda.Body as MemberExpression;
                if (memberExp != null && memberExp.Member is PropertyInfo property)
                {
                    if (!ReflectionHelper.IsSystemType(property.PropertyType))
                        isMemberClass = true;

                }
                if (!isMemberClass)
                {
                    Visit(lambda.Body);
                    return;
                }

            }

            var entity = EntityCache.GetOrAdd(lambda.ReturnType);

            var cols = entity.Fields.Select(m => $"{SqlFormatter.FormatQualifiedColumnName(lambda.ReturnType, m.ColumnName)}");

            Write(string.Join("," + Environment.NewLine, cols));


        }

        private void ResolvingGroup(Expression grpExp)
        {

            if (grpExp == null)
                return;

            var groupByExp = StripQuotes(grpExp) as LambdaExpression;

            groupCols = new List<GroupInfo>();


            if (groupByExp.Body is MemberExpression me)
            {
                groupCols.Add(new GroupInfo
                {
                    Type = me.Expression.Type,
                    Name = me.Member.Name
                });
                return;

            }

            if (groupByExp.Body.NodeType == ExpressionType.New)
            {

                NewExpression newExpression = groupByExp.Body as NewExpression;

                foreach (var exp in newExpression.Arguments)
                {

                    MemberExpression member = exp as MemberExpression;
                    groupCols.Add(new GroupInfo { Type = member.Expression.Type, Name = member.Member.Name });

                }

            }
            else
            {
                var et = EntityCache.GetOrAdd(groupByExp.ReturnType);

                foreach (var f in et.Fields)
                {
                    groupCols.Add(new GroupInfo { Type = et.EntityType, Name = f.Name });
                }

            }


        }


        protected override Expression VisitMember(MemberExpression node)
        {
            //only one member

            if (groupCols != null)
            {

                var gropCol = groupCols[0];
                Write(SqlFormatter.FormatQualifiedColumnName(gropCol.Type, gropCol.Name));
            }
            else
            {

                Write(SqlFormatter.FormatQualifiedColumnName(node.Expression.Type, node.Member.Name));
            }
            return node;
        }


        private void VisitNewOrBinding(Expression m)
        {

            if (SqlPart.Length > 0)
            {
                Write(",");

            }
            Write(Environment.NewLine);

            if (m.NodeType == ExpressionType.Call)
            {

                var tr = new AggregateTranslator(SqlFormatter);
                tr.Translate(m);
                Write(tr.SqlPart.ToString());

            }
            else
            {

                MemberExpression member = m as MemberExpression;
                Type type = member.Expression.Type;

                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    member = member.Expression as MemberExpression;
                    type = member.Expression.Type;


                }

                var name = member.Member.Name;

                if (groupCols != null)
                {
                    if (name == "Key")
                    {
                        if (groupCols.Count > 0)
                        {
                            var grp = groupCols[0];
                            type = grp.Type;
                            name = grp.Name;
                        }

                    }
                    else
                    {
                        type = groupCols.First(m => m.Name == name).Type;
                    }

                }


                Write(SqlFormatter.FormatQualifiedColumnName(type, name));
            }

            Write(" AS ");

        }


        protected override Expression VisitNew(NewExpression node)
        {
            // anonymous type
            int index = 0;
            foreach (var arg in node.Arguments)
            {

                VisitNewOrBinding(arg);
                Write(SqlFormatter.FormatColumnName(node.Members[index].Name));
                index++;

            }

            return node;

        }



        protected override MemberBinding VisitMemberBinding(MemberBinding node)
        {

            //new class 
          
            MemberAssignment assignment = node as MemberAssignment;

            VisitNewOrBinding(assignment.Expression);

            Write(SqlFormatter.FormatColumnName(node.Member.Name));

            return node;

        }







    }
}
