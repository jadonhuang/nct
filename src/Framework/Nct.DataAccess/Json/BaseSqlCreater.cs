using Microsoft.SqlServer.Management.SqlParser.SqlCodeDom;
using Nct.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Nct.DataAccess.Json.Constants;

namespace Nct.DataAccess.Json
{
    public abstract class BaseSqlCreater : ISqlCreater
    {

        private const string DefAlias = "t";
        private int index = 0;
        private int tbIndex = 0;
        private SqlInfo sqlInfo = new SqlInfo();
        private Dictionary<string, string> tableAlias = new Dictionary<string, string>();

        protected static Dictionary<string, string> SqlFuns = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
            {"min",null },
             {"max",null },
              {"sum",null },
               {"avg",null },
                {"count",null },
                 {"year",null },
                  {"month",null },
                   {"day",null },
                   {"datepart",null },
                    {"datediff",null },
                      {"convert",null },
                       {"left",null },
                        {"right",null },
                           {"substring",null },
                              {"charindex",null },
                                 {"cast",null },
                                   {"replace",null },
                                     {"abs",null },
                                       {"round",null },
                                         {"str",null },
                                           {"datename",null },
                                            {"len",null },
                                             {"lower",null },
                                                 {"upper",null },
                                                   {"stuff",null },
                                                   {"dateadd",null },
                                                     {"isdate",null },
                                                       {"ltrim",null },
                                                         {"rtrim",null },
                                                          {"isnumeric",null },
                                                               {"getdate",null },

        };



        protected virtual void CheckSqlFun(string funExp)
        {

            funExp = $"select {funExp}";

            var result = Microsoft.SqlServer.Management.SqlParser.Parser.Parser.Parse(funExp);

            if (!result.Errors.Any())
            {

                void RecursionToken(IEnumerable<SqlCodeObject> sqlCodeObjects)
                {

                    foreach (var codeObject in sqlCodeObjects)
                    {

                        if (codeObject is SqlBuiltinScalarFunctionCallExpression sqlBuiltinScalarFunction)
                        {

                            if (!SqlFuns.ContainsKey(sqlBuiltinScalarFunction.FunctionName))
                                throw new JsonException($"不支持函数:{sqlBuiltinScalarFunction.FunctionName}");

                        }
                        else if (codeObject is SqlScalarSubQueryExpression)
                        {
                            throw new JsonException("函数中不支持子查询");
                        }

                        if (codeObject.Children.Any())
                            RecursionToken(codeObject.Children);

                    }


                }

                var child = result.Script.Children;

                if (child.Count() != 1 || child.First().Children.Count() != 1) throw new JsonException("错误的函数体");

                RecursionToken(result.Script.Children);

            }
            else
            {

                if (result.Errors.Any())
                {

                    throw new JsonException(result.Errors.First().Message);
                }

            }



        }


        protected virtual string CreateQuote(string name)
        {

            return $"[{name}]";

        }

        protected virtual string ParaPrefix { get; } = "@";

        public List<JsonTable> Tables { get; set; }

        protected virtual SqlInfo CreateInsertSql(JsonTable table)
        {

            var fValues = table.FieldValues.Where(m => m.Operator == "=");

            var fields = string.Join(",", fValues.Select(m => $"{CreateQuote(m.Name)}"));
            var values = string.Join(",", fValues.Select(m => $"{ParaPrefix}{m.Name}"));

            var sql = $"insert into {CreateQuote(table.Table)}({fields}) values ({values})";
            sqlInfo.IndentityIdSql = GetIndentitySql();
            sqlInfo.Sql = sql;
            sqlInfo.Parameters = table.FieldValues.FieldValuesToDic();

            return sqlInfo;

        }

        protected virtual SqlInfo CreateUpdateSql(JsonTable table)
        {

            StringBuilder sql = new StringBuilder();
            sql.Append("update ");

            ConditionInfo conditionInfo;

            string updateFields;

            if (table.Joins.Any())
            {

                conditionInfo = BuildCondition(table, StatementType.Update, true, DefAlias);
                StringBuilder joinSqlStr = new StringBuilder();
                List<string> whereList = new List<string>();

                int recursionNum = 0;
                BuildUpdateOrDeleteJoin(table, DefAlias, joinSqlStr, whereList, StatementType.Update, recursionNum);

                string whereStr = null;
                if (conditionInfo.Condition != null)
                {
                    whereList.Insert(0, conditionInfo.Condition.Substring(5));
                }

                if (whereList.Count > 0)
                {
                    whereStr = string.Join(" and ", whereList.Select(m => $"({m})"));
                    whereStr = $" and {whereStr}";
                }

                //sqlserver join not surport update multi tables
                updateFields = string.Join(",", table.FieldValues.Select(m => GetUpdateExpression(m, DefAlias)));

                foreach (var relate in table.AssociatedTables)
                {
                    var rTable = relate.Table;
                    if (!tableAlias.TryGetValue(rTable, out string alias))
                    {
                        throw new JsonException($"关联表{rTable}未找到");
                    }

                    updateFields += $",{DefAlias}.{CreateQuote(relate.Field)} = {alias}.{CreateQuote(relate.AssociatedField)}";

                }
                updateFields = updateFields.TrimStart(',');
                sql.Append($"{DefAlias} set {updateFields} from {CreateQuote(table.Table)} {DefAlias} {joinSqlStr} {whereStr}");

            }
            else
            {
                updateFields = string.Join(",", table.FieldValues.Select(m => GetUpdateExpression(m, null)));

                conditionInfo = BuildCondition(table, StatementType.Update);
                sql.Append($"{CreateQuote(table.Table)} set {updateFields} {conditionInfo.Condition}");


            }

            if (updateFields.IsEmpty()) throw new JsonException($"无更新字段");

            sqlInfo.Sql = sql.ToString();
            conditionInfo.Parameters.CopyToDic(sqlInfo.Parameters);
            table.FieldValues.FieldValuesToDic().CopyToDic(sqlInfo.Parameters);

            return sqlInfo;
        }

        protected virtual SqlInfo CreateDeleteSql(JsonTable table)
        {

            StringBuilder sql = new StringBuilder();
            ConditionInfo conditionInfo;
            sql.Append($"delete ");

            if (table.Joins.Any())
            {
                conditionInfo = BuildCondition(table, StatementType.Delete, true, DefAlias);
                StringBuilder joinSqlStr = new StringBuilder();
                List<string> whereList = new List<string>();

                int recursionNum = 0;
                BuildUpdateOrDeleteJoin(table, DefAlias, joinSqlStr, whereList, StatementType.Delete, recursionNum);

                string whereStr = null;
                if (conditionInfo.Condition != null)
                {
                    whereList.Insert(0, conditionInfo.Condition.Substring(5));
                }

                if (whereList.Count > 0)
                {
                    whereStr = string.Join(" and ", whereList.Select(m => $"({m})"));
                    whereStr = $" and {whereStr}";
                }

                sql.Append($"{DefAlias} from {CreateQuote(table.Table)} {DefAlias} {joinSqlStr} {whereStr}");

            }
            else
            {
                conditionInfo = BuildCondition(table, StatementType.Delete);
                sql.Append($"{CreateQuote(table.Table)} {conditionInfo.Condition}");

            }


            sqlInfo.Sql = sql.ToString();
            conditionInfo.Parameters.CopyToDic(sqlInfo.Parameters);

            return sqlInfo;

        }

        protected virtual string CreatePageSqlString(JsonTable table, string fields, string where, string groupBy, string having, string orderBy)
        {
            throw new NotImplementedException();
        }

        protected virtual string CreateTopSqlString(JsonTable table, string fields, string where, string groupBy, string having, string orderBy)
        {

            throw new NotImplementedException();
        }

        private string GetFieldStr(Field field)
        {
            if (field.Name == "*") return field.Name;
            return field.FunExpression.IsEmpty() ? CreateQuote(field.Name) : $"{field.FunExpression}";
        }

        private string GetAliasStr(Field field)
        {
            return field.Alias.IsEmpty() ? "" : $"{CreateQuote(field.Alias)}";
        }


        private static string ReplaceTablePlaceholder(string str, string table)
        {
            return str.Replace("_table_", table);
        }


        private string GetFullFieldName(string tableAliasName, Field field)
        {
            string fullFieldName;
            if (field.FunExpression.IsEmpty())
            {
                fullFieldName = $"{tableAliasName}.{GetFieldStr(field)} {GetAliasStr(field)}";
            }
            else
            {
                field.FunExpression = ReplaceTablePlaceholder(field.FunExpression, tableAliasName);
                fullFieldName = $"{GetFieldStr(field)} {GetAliasStr(field)}";
            }

            return fullFieldName;

        }


        private List<string> GetFullFieldNames(string tableAliasName, List<Field> fields)
        {

            var fieldList = new List<string>();
            foreach (var field in fields)
            {
                string fullFieldName = GetFullFieldName(tableAliasName, field);
                fieldList.Add(fullFieldName);

            }
            return fieldList;
        }

        protected virtual SqlInfo CreateSelectSql(JsonTable table)
        {

            var conditionInfo = BuildCondition(table, StatementType.Select);
            var tableName = "";
            var fields = "*";
            if (table.Joins.Count>0)
            {

                var sql = new StringBuilder();
                var fieldList = new List<string>();
                int recursionNum = 0;
                BuildSelectJoin(table, DefAlias, sql, fieldList, recursionNum);
                if (table.Fields.Count == 0)
                {
                    fieldList.Add($"{DefAlias}.*");

                }
                else
                {
                    var fieldNames = GetFullFieldNames(DefAlias, table.Fields);
                    fieldList.AddRange(fieldNames);
                }

                var fieldsStr = string.Join(",", fieldList);

                string tableStr = $"select {fieldsStr} from {CreateQuote(table.Table)} t {sql}";
                tableName = $"({tableStr}) tt";
                table.Table = tableName;

            }
            else
            {

                if (table.Fields.Count > 0)
                {
                    fields = string.Join(",", table.Fields.Select(m => $"{GetFieldStr(m)} {GetAliasStr(m)}"));
                }
                tableName = CreateQuote(table.Table);
            }


            var havingInfo = BuildCondition(table, StatementType.Select, false);
            havingInfo.Parameters.CopyToDic(sqlInfo.Parameters);

            var groupBy = "";
            if (table.GropuBys.Count > 0)
            {
                groupBy = "group by " + string.Join(",", table.GropuBys.Select(m => $"{CreateQuote(m.Name)}"));

            }

            string orderBy = "";

            if (table.OrderBys.Count > 0)
            {
                orderBy = "order by " + string.Join(",", table.OrderBys.Select(m => $"{CreateQuote(m.Key)} {m.Value}"));
            }

            if (table.Page.HasValue)
            {

                sqlInfo.Sql = CreatePageSqlString(table, fields, conditionInfo.Condition, groupBy, havingInfo.Condition, orderBy);
                sqlInfo.ExtraSql = $"select count(1) as total from {tableName} {conditionInfo.Condition}";
            }

            else
            {

                if (table.Count.HasValue)
                {
                    sqlInfo.Sql = CreateTopSqlString(table, fields, conditionInfo.Condition, groupBy, havingInfo.Condition, orderBy);
                }
                else
                {
                    var sql = $"select {fields} from {tableName} {conditionInfo.Condition} {groupBy} {havingInfo.Condition} {orderBy}";
                    sqlInfo.Sql = sql;
                }

                sqlInfo.ExtraSql = $"select count(1) as total from {tableName} {conditionInfo.Condition} {groupBy} {havingInfo.Condition}";

            }

            conditionInfo.Parameters.CopyToDic(sqlInfo.Parameters);
            return sqlInfo;


        }


        protected virtual string GetIndentitySql()
        {

            throw new NotImplementedException();

        }

        private static string GetLogicStr(LogicType logicType)
        {
            var logic = logicType == LogicType.And ? "and" : "or";
            return logic;
        }

        protected virtual string AddParam(ConditionInfo cdtInfo, object value)
        {
            var paramName = $"{ParaPrefix}_p{index}";
            cdtInfo.Parameters[paramName] = value;
            index++;
            return paramName;

        }


        protected virtual string BuildFieldWhere(Condition condition, ConditionInfo cdtInfo, string tableAlias)
        {

            string where = null;

            if (tableAlias != null) tableAlias += ".";

            if (condition.Value == null || condition.Value.ToString() == "null")
            {
                var op = condition.Operator == "=" ? SqlKeys.ISNULL : SqlKeys.ISNOTNULL;

                if (condition.FunExpression.IsEmpty())
                    where = $"{tableAlias}{CreateQuote(condition.Field)} {op}";
                else
                    where = $"{condition.FunExpression} {op}";
            }
            else
            {
                //tableAlias is not null when update、delete
                string fieldStr = condition.FunExpression.IsEmpty() ? $"{tableAlias}{CreateQuote(condition.Field)}" : $"{ReplaceTablePlaceholder(condition.FunExpression, tableAlias)}";

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

                }
                else
                {
                    var paramName = AddParam(cdtInfo, condition.Value);
                    where = $"{fieldStr} {condition.Operator} {paramName}";

                }


            }

            return where;

        }


        protected virtual ConditionInfo BuildCondition(JsonTable table, StatementType statementType, bool isWhere = true, string tableAlias = null)
        {

            ConditionInfo cdtInfo = new ConditionInfo();

            var jsonCdts = table.Conditions.Where(m => m.IsWhere == isWhere).ToList();


            if (isWhere)
            {
                if (statementType != StatementType.Update)
                {
                    foreach (var fv in table.FieldValues)
                    {
                        JsonCondition jsonCdt = new JsonCondition();
                        jsonCdt.Tag = fv.Name;

                        var cdt = new Condition { Field = fv.Name, Type = LogicType.And, Operator = "=", Value = fv.Value };
                        jsonCdt.Conditions.Add(cdt);
                        jsonCdts.Add(jsonCdt);

                    }

                }


                if (statementType == StatementType.Select)
                {

                    foreach (var ass in table.AssociatedTables)
                    {

                        JsonCondition jsonCdt = new JsonCondition();
                        jsonCdt.Tag = ass.Field;

                        Condition cdt;
                        if (ass.Condition != null)
                        {
                            cdt = ass.Condition;
                            var paramName = AddParam(cdtInfo, ass.Value);
                            cdt.FunExpression = ass.Condition.FunExpression.Replace(ass.Tag, paramName);

                        }
                        else
                        {
                            cdt = new Condition { Field = ass.Field, Type = LogicType.And, Operator = "=", Value = ass.Value };
                            if (ass.Value != null && ass.Value is string strValue && strValue.Contains(","))
                            {
                                cdt.Operator = SqlKeys.IN;
                                cdt.Value = strValue.SplitString().ToList();
                            }

                        }

                        jsonCdt.Conditions.Add(cdt);
                        jsonCdts.Add(jsonCdt);

                    }

                }


            }


            List<ConditionSql> conditionSqls = new List<ConditionSql>();

            foreach (var jsonCdt in jsonCdts)
            {
                if (jsonCdt.Conditions.Count == 0) continue;

                StringBuilder sql = new StringBuilder();
                var jsonCdtIndex = 0;
                sql.Append("(");
                foreach (var cdt in jsonCdt.Conditions)
                {

                    if (cdt is ConditionCollection collection)
                    {

                        var cdtCount = collection.Conditions.Count;

                        if (cdtCount == 0) continue;

                        if (jsonCdtIndex > 0)
                        {
                            var logic = GetLogicStr(collection.Conditions[0].Type);
                            sql.Append($" {logic}");
                        }

                        int cdtIndex = 0;
                        sql.Append("(");
                        foreach (var condition in collection.Conditions)
                        {
                            if (condition.FunExpression.IsNotEmpty())
                                CheckSqlFun(condition.FunExpression);

                            var logic = GetLogicStr(condition.Type);
                            string whereStr = BuildFieldWhere(condition, cdtInfo, tableAlias);

                            if (whereStr.IsNotEmpty())
                            {
                                if (cdtIndex != 0)
                                {
                                    sql.Append($" {logic}");
                                }
                                sql.Append($" {whereStr}");
                                cdtIndex++;
                            }

                        }

                        sql.Append(')');

                    }
                    else
                    {

                        var condition = cdt as Condition;
                        if (condition.FunExpression.IsNotEmpty())
                            CheckSqlFun(condition.FunExpression);

                        string whereStr = BuildFieldWhere(condition, cdtInfo, tableAlias);
                        if (whereStr.IsNotEmpty())
                        {
                            if (jsonCdtIndex > 0)
                            {
                                var logic = GetLogicStr(condition.Type);
                                sql.Append($" {logic} ");
                            }
                            sql.Append($" {whereStr}");
                        }
                    }

                    jsonCdtIndex++;

                }
                sql.Append(")");
                if (sql.Length > 2)
                    conditionSqls.Add(new ConditionSql { Tag = jsonCdt.Tag, Sql = sql.ToString() });

            }

            string combine = table.Combine;
            StringBuilder where = new StringBuilder();

            if (isWhere && combine.IsNotEmpty())
            {

                var matches = Regex.Matches(combine, @"\w+(:\w+)?(\!\[\]|[\|\&]{0,1}(\{\})|~|\[\])*");

                var count = matches.Count;

                for (var i = 0; i < count; i++)
                {

                    var m = matches[i];
                    var part = m.Value;
                    var cdtSql = conditionSqls.FirstOrDefault(n => n.Tag == part);
                    if (cdtSql == null)
                    {
                        throw new JsonException($"combine中条件{part}未找到");
                    }
                    conditionSqls.Remove(cdtSql);
                    if (i == 0)
                    {
                        where.Append(combine.Substring(0, m.Index));
                    }
                    where.Append($"{cdtSql.Sql}");
                    if (i < count - 1)
                    {
                        var next = matches[i + 1];
                        var start = m.Index + m.Length;
                        where.Append(combine.Substring(start, next.Index - start));
                    }
                    else
                    {
                        where.Append(combine.Substring(m.Index + m.Length));
                    }

                }

                where = where.Replace("|", " or ").Replace("&", " and ");


            }

            if (conditionSqls.Count > 0)
            {
                if (where.Length > 0) where.Append(" and ");
                where.Append(string.Join(" and ", conditionSqls.Select(m => m.Sql)));

            }

            if (where.Length > 0)
            {
                var cdtKey = isWhere ? "where" : "having";
                cdtInfo.Condition = $"{cdtKey} {where}";

            }

            if (isWhere)
            {
                string cdt = cdtInfo.Condition;
                SqlHelper.AppendContextExtraWhere(ref cdt);
                cdtInfo.Condition = cdt;

            }

            return cdtInfo;

        }

        private void BuildSelectJoin(JsonTable table, string tbName, StringBuilder joinSqlStr, List<string> fields, int recursionNum)
        {

            recursionNum++;
            if (recursionNum > 100) throw new JsonException($"join貌似死循环了");

            foreach (var join in table.Joins)
            {

                if (join.Table == table.Table)
                    throw new JsonException($"{table.Table}自连接");

                var jsonTable = Tables.FirstOrDefault(m => m.Table == join.Table && m.ParentKey == table.ParentKey);

                if (jsonTable == null)
                    throw new JsonException($"未找到{join.Table}关联表信息");

                tbIndex++;
                var tbAliasName = $"{DefAlias}{tbIndex}";

                var cdt = BuildCondition(jsonTable, StatementType.Select);
                bool hasCdt = cdt.Condition.IsNotEmpty();
                if (jsonTable.Fields.Any())
                {
                    string fieldName;
                    foreach (var field in jsonTable.Fields)
                    {
                        if (hasCdt && field.Alias != null)
                        {
                            fieldName = $"{tbAliasName}.{CreateQuote(field.Alias)}";

                        }
                        else
                        {
                            fieldName = GetFullFieldName(tbAliasName, field);
                        }
                        fields.Add(fieldName);
                    }

                }

                string tableName = "";

                if (hasCdt)
                {
                    var fieldStr = string.Join(",", jsonTable.Fields.Select(m => $"{GetFieldStr(m)} {GetAliasStr(m)}"));
                    if (fieldStr.IsEmpty())
                    {
                        fieldStr = CreateQuote(join.JoinedField);
                    }
                    else
                    {
                        if (!jsonTable.Fields.Any(m => m.Alias == null && m.Name.IgnoreCaseEqual(join.JoinedField)))
                            fieldStr += "," + CreateQuote(join.JoinedField);
                    }

                    tableName = $"(select {fieldStr} from {CreateQuote(jsonTable.Table)} {cdt.Condition}) {tbAliasName}";

                }
                else
                {
                    tableName = $"{CreateQuote(jsonTable.Table)} {tbAliasName}";
                }

                var joinType = join.Type == JoinType.LeftJoin ? "left join" : "inner join";

                var joinLPart = $"{tbName}.{CreateQuote(join.Field)}";
                var joinRPart = $"{tbAliasName}.{CreateQuote(join.JoinedField)}";

                if (join.Condition == null)
                {
                    joinSqlStr.Append($" {joinType} {tableName} on {joinLPart} = {joinRPart}");
                }
                else
                {
                    var funExp = join.Condition.FunExpression.Replace(join.Tag, joinRPart);
                    funExp = ReplaceTablePlaceholder(funExp, tbName);
                    CheckSqlFun(funExp);
                    var paramName = AddParam(cdt, join.Condition.Value);
                    joinSqlStr.Append($" {joinType} {tableName} on {funExp} {join.Condition.Operator} {paramName}");
                }


                cdt.Parameters.CopyToDic(sqlInfo.Parameters);

                BuildSelectJoin(jsonTable, tbAliasName, joinSqlStr, fields, recursionNum);

            }



        }


        private void BuildUpdateOrDeleteJoin(JsonTable table, string tbName, StringBuilder joinSqlStr, List<string> whereList, StatementType statementType, int recursionNum)
        {

            recursionNum++;
            if (recursionNum > 100) throw new JsonException($"join貌似死循环了");

            foreach (var join in table.Joins)
            {

                if (join.Type != JoinType.InnerJoin)
                    throw new JsonException($"只支持inner join");

                if (join.Table == table.Table)
                    throw new JsonException($"{table.Table}自连接");

                var jsonTable = Tables.FirstOrDefault(m => m.Table == join.Table && m.ParentKey == table.ParentKey);

                if (jsonTable == null)
                    throw new JsonException($"未找到{join.Table}关联表信息");


                tbIndex++;
                var tbAliasName = $"{DefAlias}{tbIndex}";

                tableAlias[join.Table] = tbAliasName;

                var cdt = BuildCondition(jsonTable, statementType, true, tbAliasName);

                string tableName = tableName = $"{CreateQuote(jsonTable.Table)} {tbAliasName}";

                if (cdt.Condition.IsNotEmpty())
                {
                    whereList.Add(cdt.Condition.Substring(5));

                }

                var joinType = "inner join";


                var joinLPart = $"{tbName}.{CreateQuote(join.Field)}";
                var joinRPart = $"{tbAliasName}.{CreateQuote(join.JoinedField)}";


                if (join.Condition == null)
                {
                    joinSqlStr.Append($" {joinType} {tableName} on {joinLPart} = {joinRPart}");
                }
                else
                {
                    var funExp = join.Condition.FunExpression.Replace(join.Tag, joinRPart);
                    funExp = ReplaceTablePlaceholder(funExp, tbName);
                    CheckSqlFun(funExp);
                    var paramName = AddParam(cdt, join.Condition.Value);
                    joinSqlStr.Append($" {joinType} {tableName} on {funExp} {join.Condition.Operator} {paramName}");
                }

                cdt.Parameters.CopyToDic(sqlInfo.Parameters);

                BuildUpdateOrDeleteJoin(jsonTable, tbAliasName, joinSqlStr, whereList, statementType, recursionNum);

            }



        }

        private string GetUpdateExpression(FieldValue fieldValue, string tableAlias)
        {

            if (tableAlias != null) tableAlias += ".";

            if (fieldValue.Operator == "=")
            {

                return $"{tableAlias}{CreateQuote(fieldValue.Name)} = {ParaPrefix}{fieldValue.Name}";
            }

            var fieldName = CreateQuote(fieldValue.Name);
            return $"{tableAlias}{fieldName} = {tableAlias}{fieldName} {fieldValue.Operator} {ParaPrefix}{fieldValue.Name}"; //a=a+1 etc
        }




        public virtual SqlInfo CreateSql(JsonTable table, StatementType statementType)
        {

            switch (statementType)
            {
                case StatementType.Insert:
                    sqlInfo = CreateInsertSql(table);

                    break;

                case StatementType.Update:
                    sqlInfo = CreateUpdateSql(table);
                    break;

                case StatementType.Delete:
                    sqlInfo = CreateDeleteSql(table);
                    break;

                default:
                    if (table.Fields.Any())
                    {
                        foreach (var f in table.Fields)
                        {
                            if (f.FunExpression.IsEmpty()) continue;
                            CheckSqlFun(f.FunExpression);

                        }

                    }
                    sqlInfo = CreateSelectSql(table);
                    break;
            }
            return sqlInfo;

        }




    }
}
