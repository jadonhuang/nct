using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using static Nct.DataAccess.DynamicQuery.Constants;

namespace Nct.DataAccess.DynamicQuery
{
    public class DynamicSqlInfo
    {


        public IDictionary<string, object> Arguments { get; set; } =  new Dictionary<string,object>(StringComparer.OrdinalIgnoreCase);

        public string Condition { get; set; } = string.Empty;

        public string SqlType { get; set; } = SqlKeyword.Select;

        public int? TopNum { get; set; }

        public int? SkipNum { get; set; }

        public string TableName { get; set; }

        public string OrderBy { get; set; }

        public string Fields { get; set; }

        public bool NeedAudit { get; set; }

        public object Data { get; set; }

        public List<string> FieldList { get; set; } = [];


        public override string ToString()
        {

            if (string.IsNullOrWhiteSpace(TableName)) throw new ArgumentException("请指定表名");

            var sql = new StringBuilder();
            sql.Append(SqlType);

            if (SqlType == SqlKeyword.Select)
            {
                var field = Fields ?? "*";
                if (!SkipNum.HasValue)
                {
                    if (TopNum > 0)
                    {
                        sql.AppendFormat(" {0} {1} ", SqlKeyword.Top, TopNum);
                    }
                    sql.AppendFormat(" {0} ", field);
                    sql.AppendFormat(" {0}", SqlKeyword.From);
                    sql.AppendFormat(" {0}", TableName);
                    if (!string.IsNullOrEmpty(Condition))
                    {
                        sql.AppendFormat(" {0} {1}", SqlKeyword.Where, Condition);
                    }

                    if (!string.IsNullOrEmpty(OrderBy))
                    {
                        sql.AppendFormat(" {0} {1}", SqlKeyword.OrderBy, OrderBy);
                    }
                }
                else
                {
                    if(string.IsNullOrWhiteSpace(OrderBy)) throw new ArgumentException("请指定排序");


                    var rowIndex = SqlHelper.RowIndexName;
                     
                    sql.AppendFormat(" {0}", field);
                    sql.AppendFormat(",{0} {1}({2} {3}) {4}", SqlKeyword.RowNumber,SqlKeyword.Over,SqlKeyword.OrderBy,OrderBy, rowIndex);
                    sql.AppendFormat(" {0}", SqlKeyword.From);
                    sql.AppendFormat(" {0}", TableName);
                    if (!string.IsNullOrEmpty(Condition))
                    {
                        sql.AppendFormat(" {0} {1}", SqlKeyword.Where, Condition);
                    }

                    sql.Insert(0, SqlKeyword.Select+$" {field} {SqlKeyword.From}(");
                    sql.Append(")tb");
                    sql.AppendFormat(" {0} {1}>{2}", SqlKeyword.Where, rowIndex, SkipNum);
                    if (TopNum > 0)
                        sql.AppendFormat(" {0} {1}<={2}", SqlOperator.AndAlso, rowIndex, SkipNum + TopNum);

                }

            }

            else if (SqlType == SqlKeyword.Delete)
            {

                sql.AppendFormat(" {0}", SqlKeyword.From);
                sql.AppendFormat(" {0}", TableName);
                if (!string.IsNullOrEmpty(Condition))
                {
                    sql.AppendFormat(" {0} {1}", SqlKeyword.Where, Condition);
                }

            }
            else if (SqlType == SqlKeyword.Insert)
            {
        
                sql.AppendFormat(" {0}", TableName);
                sql.AppendFormat(" {0}", Fields);
               
            }
            else if (SqlType == SqlKeyword.Update)
            {

                sql.AppendFormat(" {0}", TableName);
                sql.AppendFormat(" set {0}", Fields);
                if (!string.IsNullOrEmpty(Condition))
                {
                    sql.AppendFormat(" {0} {1}", SqlKeyword.Where, Condition);
                }


            }

            return sql.ToString();

        }




    }
}
