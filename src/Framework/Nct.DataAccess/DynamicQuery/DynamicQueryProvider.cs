using Nct.Ioc;
using Nct.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using static Nct.DataAccess.DynamicQuery.Constants;


namespace Nct.DataAccess.DynamicQuery
{
    public class DynamicQueryProvider : IQueryProvider
    {


        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new DynamicQueryable<TElement>(expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }

        private Expression Eval(Expression expression)
        {
            PartialEvaluator evaluator = new PartialEvaluator();
            Expression evaluatedExpression = evaluator.Eval(expression);
            return evaluatedExpression;
        }

        private DynamicRow DicToFieldValue(IDictionary<string, object> dic)
        {
            DynamicRow row = new DynamicRow();

            foreach (var key in dic.Keys)
            {
                DynamicFieldValue fieldValue;
                var value = dic[key];
                if (value == null)
                {
                    fieldValue = DynamicFieldValue.Null;
                }
                else
                {
                    switch (value.GetType().ToString())
                    {
                        case "System.Int64":
                            fieldValue = (long)value;
                            break;
                        case "System.Int32":
                            fieldValue = (int)value;
                            break;
                        case "System.Int16":
                            fieldValue = (short)value;
                            break;

                        case "System.Float":
                            fieldValue = (float)value;
                            break;
                        case "System.Decimal":
                            fieldValue = (decimal)value;
                            break;
                        case "System.Double":
                            fieldValue = (double)value;
                            break;

                        case "System.DateTime":
                            fieldValue = (DateTime)value;
                            break;

                        case "System.String":
                            fieldValue = (string)value;
                            break;
                        default:
                            fieldValue = value.ToString();
                            break;


                    }


                }

                row[key] = fieldValue;

            }

            return row;
        }

        public TResult Execute<TResult>(Expression expression)
        {

            var type = typeof(TResult);

            var sqlBuilder = new SqlBuilder();
            sqlBuilder.Visit(Eval(expression));
            sqlBuilder.Build();


            var sql = sqlBuilder.SqlInfo.ToString();
            var args = sqlBuilder.SqlInfo.Arguments;



#if DEBUG
            Console.WriteLine(sql);
            Console.WriteLine(args.ToJson());
#endif


            if (sql.StartsWith(SqlKeyword.Insert) || sql.StartsWith(SqlKeyword.Delete) || sql.StartsWith(SqlKeyword.Update))
            {
                int affect = 0;
                IocManager.Instance.Using<IDbContextProvider>(ctx =>
                {
                    affect = ctx.GetDbContext<DapperDbContext>().Database.Execute(sql, args);

                });

                return (TResult)(object)affect;

            }

            List<dynamic> list = null;

            IocManager.Instance.Using<IDbContextProvider>(ctx =>
            {
                list = ctx.GetDbContext<DapperDbContext>().Database.Query<dynamic>(sql, args);

            });

            if (type == typeof(List<DynamicRow>))
            {
                var rows = new List<DynamicRow>();
                foreach (var item in list)
                {
                    var row = DicToFieldValue(item);
                    rows.Add(row);

                }

                return (TResult)(object)rows;


            }
            else if (type == typeof(DynamicRow))
            {
                if (list.Count > 0)
                {
                    var result = list[0] as IDictionary<string, object>;
                    object row = DicToFieldValue(result);
                    return (TResult)row;

                }

            }
            else
            {
                if (list.Count > 0)
                {
                    var row = list[0] as IDictionary<string, object>;
                    foreach (var key in row.Keys)
                    {
                        if (row[key] != null)
                            return (TResult)(row[key]);
                    }

                }

            }

            return default;
        }

        public object Execute(Expression expression)
        {
            return new List<object>();
        }
    }
}
