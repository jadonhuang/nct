using Nct.DataAccess.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nct.DataAccess.Json.Constants;

namespace Nct.Module.Form.DataSources
{
    public class DataSourceJsonSqlCreater : BaseSqlCreater
    {

        protected override string BuildFieldWhere(Condition condition, ConditionInfo cdtInfo, string tableAlias)
        {

            string where;

            if (tableAlias != null) tableAlias += ".";

            if (condition.Value == null || condition.Value.ToString() == "null")
            {
                var op = condition.Operator == "=" ? SqlKeys.ISNULL : SqlKeys.ISNOTNULL;

                if (condition.FunExpression.IsEmpty())
                    where = $"{tableAlias}{CreateQuote(condition.Field)} {op}";
                else
                    where = $"{condition.FunExpression} {op}";
                cdtInfo.Parameters[condition.Field] = null;
            }
            else
            {

                string fieldStr = condition.FunExpression.IsEmpty() ? $"{tableAlias}{CreateQuote(condition.Field)}" : $"{condition.FunExpression}";

                if (condition.Operator == SqlKeys.IN || condition.Operator == SqlKeys.NOTIN)
                {
                    if (condition.Value is not IEnumerable<object> values) return null;

                    var paramNames = new List<string>();
                    foreach (var value in values)
                    {
                        var paramName = AddParam(cdtInfo, value);
                        paramNames.Add(paramName);
                    }
                    var paramStr = string.Join(",", paramNames);
                    where = $"{fieldStr} {condition.Operator} ({paramStr})";
                    cdtInfo.Parameters[condition.Field] = string.Join(",", values);
                }
                else
                {
                    var paramName = AddParam(cdtInfo, condition.Value);
                    where = $"{fieldStr} {condition.Operator} {paramName}";

                    var key = condition.Field;

                    if (key != null)
                    {
                        if (cdtInfo.Parameters.ContainsKey(key))
                            key += "2";

                        cdtInfo.Parameters[key] = condition.Value;

                    }
                }


            }

            return where;
        }


        public ConditionInfo GetConditionInfo(JsonTable table, StatementType statementType = StatementType.Select, bool isWhere = true, string tableAlias = "t")
        {
            return base.BuildCondition(table, statementType, isWhere, tableAlias);

        }


    }
}
