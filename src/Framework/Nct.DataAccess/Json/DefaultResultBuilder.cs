using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Nct.DataAccess.Json.Constants;
using Nct.Extentions;

namespace Nct.DataAccess.Json
{
    public class DefaultResultBuilder : IResultBuilder
    {



        private readonly ISqlJsonParser _sqlJsonParser;
        private readonly ISqlCreater _sqlCreater;
        private readonly ISqlExecutor _sqlExecutor;

        public ISqlJsonPrivacyChecker SqlJsonPrivacyChecker { get; set; }

        public DefaultResultBuilder(ISqlJsonParser sqlJsonParser, ISqlCreater sqlCreater, ISqlExecutor sqlExecutor)
        {
            _sqlJsonParser = sqlJsonParser;
            _sqlCreater = sqlCreater;
            _sqlExecutor = sqlExecutor;
            SqlJsonPrivacyChecker = NullSqlJsonPrivacyChecker.Instance;
        }

        protected virtual List<JsonTable> GetTables(string json, StatementType statementType)
        {
            if (json.IsEmpty()) return new List<JsonTable>();
            return _sqlJsonParser.Parse(json);

        }



        private object GetDicValue(IDictionary<string, object> dic, string key, string subKey, string parentKey)
        {

            if (parentKey != null)
            {
                if (!dic.ContainsKey(parentKey)) return null;

                if (!(dic[parentKey] is IDictionary<string, object> dic2))
                {
                    if (dic[parentKey] is List<IDictionary<string, object>> dicList && dicList.Count > 0)
                        dic = dicList[dicList.Count - 1];
                    else return null;

                }
                else
                {
                    dic = dic2;
                }
            }


            if (dic.TryGetValue(key, out object value))
            {
                if (value is IDictionary<string, object> subDic)
                {
                    if (subDic.TryGetValue(subKey, out value))
                        return value;

                }

            }

            return null;
        }

        private async Task<int> GetCountResult(JsonTable table)
        {
            var sql = _sqlCreater.CreateSql(table, StatementType.Select);
            var row = (IDictionary<string, object>)((await _sqlExecutor.Query(sql.ExtraSql, sql.Parameters))[0]);
            var value = Convert.ToInt32((row.Values.First()));
            return value;

        }

        private bool IsJoinedTable(JsonTable table, List<JsonTable> tables)
        {

            return tables.Any(m => m.Joins.Any(n => n.Table == table.Table));



        }

        private async Task<object> GetSingleTableResult(JsonTable table, IDictionary<string, object> root, bool multiRows)
        {

            if (!multiRows) table.Count = 1;

            bool hasAssValue = true;

            foreach (var ass in table.AssociatedTables)
            {
                object assValue = null;

                string parentKey = null;

                if (ass.ParentKey == JsonKeys.ARRAY)
                {
                    parentKey = ass.ParentKey;
                }
                else if (ass.ParentKey == "/")
                {
                    if (table.SelfKey.EndsWith(JsonKeys.ARRAY))
                    {
                        parentKey = table.SelfKey;
                    }
                    else
                    {
                        parentKey = table.ParentKey;
                    }

                }

                assValue = GetDicValue(root, ass.Table, ass.AssociatedField, parentKey);

                if (assValue == null)
                {
                    hasAssValue = false;
                    break;
                }
                ass.Value = assValue;
            }

            if (!hasAssValue)
            {
                return null;
            }

            var sql = _sqlCreater.CreateSql(table, StatementType.Select);
            var list = await _sqlExecutor.Query(sql.Sql, sql.Parameters);

            if (multiRows)
            {
                return list;
            }
            else
            {
                return list.FirstOrDefault();
            }

        }

        private string GetResultKey(JsonTable table)
        {
            if (table.TableAlias.IsNotEmpty())
                return table.TableAlias;
            return table.SelfKey;


        }


        protected virtual async Task<IDictionary<string, object>> BuildSelectResult(List<JsonTable> tables)
        {

            IDictionary<string, object> result = new Dictionary<string, object>();

            var parentKeys = tables.Select(m => m.ParentKey).Distinct().ToList();

            foreach (var parentKey in parentKeys)
            {
                if (parentKey == JsonKeys.ARRAY)
                {
                    var arrayTables = tables.Where(m => m.ParentKey == JsonKeys.ARRAY).ToList();

                    if (arrayTables.Count > 0)
                    {

                        List<IDictionary<string, object>> arrayRows = new List<IDictionary<string, object>>();

                        var mainTable = arrayTables[0];

                        if (mainTable.Count == null)
                            mainTable.Count = 10;

                        if (mainTable.Query > 0)
                        {
                            var total = await GetCountResult(mainTable);
                            result[JsonKeys.TOTAL] = total;
                            if (mainTable.Query == 1) break;
                        }

                        result[JsonKeys.ARRAY] = arrayRows;

                        var data = await GetSingleTableResult(mainTable, result, true);

                        var list = data as List<dynamic>;

                        if (list == null) continue;

                        foreach (var row in list)
                        {
                            var item = new Dictionary<string, object>();
                            item[GetResultKey(mainTable)] = row;
                            arrayRows.Add(item);
                            for (var i = 1; i < arrayTables.Count; i++)
                            {
                                var subTable = arrayTables[i];
                                if (IsJoinedTable(subTable, arrayTables)) continue;

                                subTable.Page = null;
                                bool multiRows = subTable.SelfKey.EndsWith(JsonKeys.ARRAY);
                                data = await GetSingleTableResult(subTable, result, multiRows);
                                if (data == null) continue;
                                item[GetResultKey(subTable)] = data;
                            }


                        }



                    }
                }
                else
                {
                    var notArrayTables = tables.Where(m => m.ParentKey != JsonKeys.ARRAY).ToList();

                    foreach (var tb in notArrayTables)
                    {
                        if (IsJoinedTable(tb, notArrayTables)) continue;
                        bool multiRows = tb.SelfKey.EndsWith(JsonKeys.ARRAY);
                        if (tb.Query > 0)
                        {
                            var total = await GetCountResult(tb);
                            result[JsonKeys.TOTAL] = total;
                            if (tb.Query == 1) continue;
                        }
                        var data = await GetSingleTableResult(tb, result, multiRows);
                        if (data == null) continue;
                        result[GetResultKey(tb)] = data;
                    }


                }

            }


            return result;

        }

        private async Task<IDictionary<string, object>> BuildDeleteOrUpdateResult(List<JsonTable> tables, StatementType statementType)
        {

            IDictionary<string, object> result = new Dictionary<string, object>();

            for (var i = 0; i < tables.Count; i++)
            {

                var table = tables[i];

                //if (table.AssociatedTables.Any()) continue;
                if (table.SelfKey.EndsWith(JsonKeys.ARRAY)) continue;
                if (IsJoinedTable(table, tables)) continue;

                var sql = _sqlCreater.CreateSql(table, statementType);
                var num = await _sqlExecutor.Execute(sql.Sql, sql.Parameters);

                if (result.TryGetValue(table.Table, out object value))
                {
                    var dic = value as Dictionary<string, int>;
                    dic[JsonKeys.COUNT] += num;

                }
                else
                {
                    result[table.Table] = new Dictionary<string, int> { { JsonKeys.COUNT, num } };

                }

            }
            return result;


        }

        protected virtual Task<IDictionary<string, object>> BuildDeleteResult(List<JsonTable> tables)
        {

            return BuildDeleteOrUpdateResult(tables, StatementType.Delete);

        }


        protected virtual Task<IDictionary<string, object>> BuildUpdateResult(List<JsonTable> tables)
        {

            return BuildDeleteOrUpdateResult(tables, StatementType.Update);

        }

        protected async virtual Task<IDictionary<string, object>> BuildInsertResult(List<JsonTable> tables)
        {

            IDictionary<string, object> result = new Dictionary<string, object>();
            Dictionary<string, List<FieldValue>> tableFieldValues = new Dictionary<string, List<FieldValue>>(StringComparer.OrdinalIgnoreCase);

            for (var i = 0; i < tables.Count; i++)
            {
                var table = tables[i];

                foreach (var ass in table.AssociatedTables)
                {
                    if (!tableFieldValues.TryGetValue(ass.Table, out List<FieldValue> values))
                    {
                        throw new JsonException($"未找到[{ass.Table}]关联数据，注意json表的先后顺序");
                    }

                    var value = values.FirstOrDefault(m => m.Name.IgnoreCaseEqual(ass.AssociatedField));
                    if (value == null)
                        throw new JsonException($"未找到[{ass.AssociatedField}]关联字段");

                    FieldValue fieldValue = new FieldValue { Name = ass.Field, Value = value.Value, Operator = value.Operator };

                    table.FieldValues.Add(fieldValue);

                }

                var sql = _sqlCreater.CreateSql(table, StatementType.Insert);
                var num = await _sqlExecutor.Execute(sql.Sql, sql.Parameters);

                var idField = table.FieldValues.FirstOrDefault(m => m.Name.IgnoreCaseEqual(JsonKeys.ID));
                if (idField != null)
                {
                    if (idField.Operator == "+")
                    {
                        var identity = await _sqlExecutor.Query(sql.IndentityIdSql, null);
                        IDictionary<string, object> dic = identity.First() as IDictionary<string, object>;
                        if (dic.Values.Any())
                            idField.Value = dic.Values.First();

                    }

                }

                if (table.ParentKey == JsonKeys.ARRAY)
                {
                    if (result.TryGetValue(table.ParentKey, out object value))
                    {

                        List<Dictionary<string, object>> list = value as List<Dictionary<string, object>>;
                        Dictionary<string, object> dic = new Dictionary<string, object> { { table.Table, Utils.FieldValuesToDic(table.FieldValues) } };
                        list.Add(dic);

                    }
                    else
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object> { { table.Table, Utils.FieldValuesToDic(table.FieldValues) } };
                        List<Dictionary<string, object>> list = new List<Dictionary<string, object>> { dic };
                        result[table.ParentKey] = list;


                    }

                }
                else
                {
                    result[table.Table] = Utils.FieldValuesToDic(table.FieldValues);

                }
                tableFieldValues[table.Table] = table.FieldValues;

            }

            return result;

        }




        public virtual async Task<IDictionary<string, object>> Build(string json, StatementType statementType)
        {

            var tables = GetTables(json, statementType);
            if (tables.Count == 0) return null;

            foreach (var table in tables)
            {

                var statement = statementType == StatementType.None ? table.Statement.Value : statementType;

                var checkResult = await SqlJsonPrivacyChecker.Check(table, statement);
                if (!checkResult.Success)
                    throw new JsonException(checkResult.Message);
            }

            _sqlCreater.Tables = tables;

            Exception excpt = null;

            async Task<IDictionary<string, object>> GetResult(List<JsonTable> tableList, StatementType type)
            {

                if (type == StatementType.Select)
                {

                    return await BuildSelectResult(tableList);

                }
                else if (type == StatementType.Delete)
                {

                    return await BuildDeleteResult(tableList);


                }
                else if (type == StatementType.Update)
                {

                    return await BuildUpdateResult(tableList);

                }
                else if (type == StatementType.Insert)
                {

                    return await BuildInsertResult(tableList);

                }

                return null;

            }


            try
            {

                await _sqlExecutor.Begin();

                if (statementType != StatementType.None)
                {

                    return await GetResult(tables, statementType);

                }

                else
                {
                    IDictionary<string, object> result = new Dictionary<string, object>();
                    var types = tables.Where(m => m.Statement.HasValue).Select(m => m.Statement.Value).Distinct().ToList();
                    foreach (var type in types)
                    {

                        var tableList = tables.Where(m => m.Statement == type).ToList();
                        result[type.ToString()] = await GetResult(tableList, type);

                    }
                    return result;

                }

            }
            catch (Exception ex)
            {
                excpt = ex;
                throw;


            }
            finally
            {

                await _sqlExecutor.End(excpt);
            }




        }
    }
}
