
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Nct.LinqQuery
{
    public class QueryBuilder : ExpressionVisitor
    {

        private SqlFormatter sqlFormatter;
        private Expression expression;

        private List<Expression> whereExpressions = new List<Expression>();
        private List<Expression> orderExpressions = new List<Expression>();
        private SelectExpression selectExpression;
        private List<Expression> joinExpressions = new List<Expression>();
        private Expression aggregateExpression;
        private Expression groupExpression;
        private Expression updaterExpression;

        private SqlInfo sqlInfo;

        public QueryBuilder(Expression expression, SqlFormatter formatter)
        {

            Expression evaluatedExpression = new PartialEvaluator().Eval(expression);
            this.expression = evaluatedExpression;
            sqlFormatter = formatter;


        }

        public SqlInfo Build()
        {
            sqlInfo = new SqlInfo();

            base.Visit(expression);

            BaseTranslator translator;

            if (whereExpressions.Count > 0)
            {
                translator = new WhereTranslator(sqlFormatter);
                foreach (var exp in whereExpressions)
                {
                    translator.Translate(exp);
                }
                sqlInfo.Where = translator.SqlPart.ToString();

            }

            if (orderExpressions.Count > 0)
            {
                translator = new OrderTranslator(sqlFormatter);
                foreach (var exp in orderExpressions)
                {

                    translator.Translate(exp);
                }
                sqlInfo.Order = translator.SqlPart.ToString();

            }

            if (selectExpression != null)
            {
                if (groupExpression != null)
                {
                    selectExpression.GroupBy = groupExpression;
                }
                translator = new SelectTranslator(sqlFormatter);
                translator.Translate(selectExpression);
                sqlInfo.Select = translator.SqlPart.ToString();

            }

            if (groupExpression != null)
            {

                translator = new GroupByTranslator(sqlFormatter);
                translator.Translate(groupExpression);
                sqlInfo.GroupBy = translator.SqlPart.ToString();

            }

            if (aggregateExpression != null)
            {
                sqlInfo.TopNum = null;
                sqlInfo.SkipNum = null;
                translator = new AggregateTranslator(sqlFormatter);
                translator.Translate(aggregateExpression);
                sqlInfo.Select = translator.SqlPart.ToString();
                sqlInfo.IsAggregate = true;

            }

            if (updaterExpression != null)
            {
                translator = new UpdaterTranslator(sqlFormatter);
                translator.Translate(updaterExpression);
                sqlInfo.Select = translator.SqlPart.ToString();

            }

            if (joinExpressions.Count > 0)
            {
                translator = new JoinTranslator(sqlFormatter);
                foreach (var exp in joinExpressions)
                {
                    translator.Translate(exp);
                }
                sqlInfo.Join = translator.SqlPart.ToString();

            }


            if (sqlInfo.Table == null && expression is ConstantExpression ce)
            {
                SetSqlEntityInfo(ce);

            }

            sqlInfo.Parameters = sqlFormatter.Parameters;
            return sqlInfo;
        }



        private void SetSqlEntityInfo(ConstantExpression ce)
        {
            dynamic value = ce.Value;
            var elType = value.ElementType;
            sqlInfo.EntityType = elType;
            sqlInfo.Table = sqlFormatter.FormatTableNameAsAlias(elType);
            sqlInfo.TableAlias = sqlFormatter.GetOrSetTableNameAlias(elType);

        }


        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable) || node.Method.DeclaringType == typeof(QueryExtensions))
            {

                if (sqlInfo.Table == null && node.Arguments[0] is ConstantExpression ce)
                {
                    SetSqlEntityInfo(ce);
                }

                var name = node.Method.Name;

                switch (name)
                {
                    case "Where":
                        whereExpressions.Insert(0, new WhereExpression { Source = node.Arguments[0], Where = node.Arguments[1] });
                        break;

                    case "FirstOrDefault":
                    case "First":
                        sqlInfo.TopNum = 1;
                        if (node.Arguments.Count == 2)
                        {
                            whereExpressions.Insert(0, node.Arguments[1]);
                        }
                        break;

                    case "Select":
                        selectExpression = new SelectExpression { Select = node.Arguments[1] };
                        break;

                    case "Join":
                    case "GroupJoin":

                        for (var index = 0; index < 2; index++)
                        {
                            if (node.Arguments[index].NodeType == ExpressionType.Call)
                            {
                                var methodExp = node.Arguments[index] as MethodCallExpression;

                                for (int i = 1; i < methodExp.Arguments.Count; i++)
                                {
                                    var arg = methodExp.Arguments[i];
                                    if (arg is MethodCallExpression method && method.Method.Name == "Where")
                                    {

                                        whereExpressions.Add(arg);

                                    }

                                }

                            }
                        }


                        joinExpressions.Insert(0, node);
                        break;

                    case "GroupBy":
                        groupExpression = node.Arguments[1];
                        break;

                    case "OrderBy":
                    case "OrderByDescending":
                        var orderType = name == "OrderByDescending" ? OrderType.Desc : OrderType.Asc;
                        var exp = new OrderExpression { OrderType = orderType, OrderBy = node.Arguments[1], Source = node.Arguments[0] };
                        orderExpressions.Insert(0, exp);
                        break;

                    case "Take":
                        int num = (int)(node.Arguments[1] as ConstantExpression).Value;
                        sqlInfo.TopNum = num;
                        break;

                    case "Delete":
                        if (node.Arguments.Count == 2)
                        {
                            sqlInfo.Entity = ((ConstantExpression)node.Arguments[1]).Value;
                        }
                        sqlInfo.Type = SqlType.Delete;
                        break;

                    case "Update":
                        if (node.Arguments.Count == 2)
                        {
                            if (node.Arguments[1] is ConstantExpression)
                            {
                                sqlInfo.Entity = ((ConstantExpression)node.Arguments[1]).Value;
                            }
                            else
                            {
                                updaterExpression = node.Arguments[1];
                            }


                        }
                        sqlInfo.Type = SqlType.Update;
                        break;

                    case "Insert":
                        sqlInfo.Entity = ((ConstantExpression)node.Arguments[1]).Value;
                        sqlInfo.Type = SqlType.Insert;
                        break;

                    case "Skip":
                        num = (int)(node.Arguments[1] as ConstantExpression).Value;
                        sqlInfo.SkipNum = num;
                        break;

                    case "Distinct":
                        sqlInfo.IsDistinct = true;
                        break;

                    case "Count":
                    case "LongCount":
                    case "Average":
                    case "Sum":
                    case "Min":
                    case "Max":
                        aggregateExpression = node;
                        break;

                    case "WithNoLock":
                        sqlInfo.LockMode = "NOLOCK";
                        break;

                    case "Last":
                    case "LastOrDefault":
                    case "Zip":
                        throw new NotSupportedException($"{name} not supported");





                }



            }

            return base.VisitMethodCall(node);
        }
    }
}
