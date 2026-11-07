using Nct.Extentions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using static Nct.DataAccess.Json.Constants;


namespace Nct.DataAccess.Json
{
    public class SqlJsonParser : ISqlJsonParser
    {

        private readonly List<JsonTable> tables = [];

        private static bool IsTable(string str)
        {
            return str.IsNotEmpty() && char.IsUpper(str[0]);
        }


        private static int? GetPage(JsonElement ele)
        {
            var prop = ele.SafeGetProperty(JsonKeys.PAGE);

            if (prop.ValueKind == JsonValueKind.Number)
                return prop.GetInt32();

            return null;

        }


        private static int? GetCount(JsonElement ele)
        {
            var prop = ele.SafeGetProperty(JsonKeys.COUNT);

            if (prop.ValueKind == JsonValueKind.Number)
                return prop.GetInt32();

            return null;

        }

        private static int GetQuery(JsonElement ele)
        {
            var prop = ele.SafeGetProperty(JsonKeys.QUERY);

            if (prop.ValueKind == JsonValueKind.Number)
                return prop.GetInt32();

            return 0;

        }

        private static void CheckName(string field)
        {
            if (!field.IsName() && field != "*")
                throw new JsonException($"{field} 格式不正确,注意空格");

        }

        private static string[] GetFieldAndAlias(string fieldStr)
        {
            var index = fieldStr.LastIndexOf(':');
            if (index == -1) return [fieldStr, null];

            if (index == fieldStr.Length - 1)
                throw new JsonException($"{fieldStr} 格式不正确");

            var field = fieldStr.Substring(0, index);
            var alias = fieldStr.Substring(index + 1);

            return [field.Trim(), alias.Trim()];
        }



        private static List<Field> GetFields(string fieldStr, bool isGroup)
        {

            var fields = new List<Field>();

            if (fieldStr.IsEmpty()) return fields;

            foreach (var fieldPart in fieldStr.SplitString(";"))
            {

                var fieldPart2 = fieldPart.Trim();
                if (fieldPart2.IsSqlFun())
                {

                    if (isGroup) continue;

                    var fieldObj = new Field();
                    var nameAndAlias = GetFieldAndAlias(fieldPart2);

                    var fieldName = nameAndAlias[0];
                    if (!fieldName.EndsWith(')'))
                        throw new JsonException($"错误的函数体:{fieldName}");

                    var fieldAlis = nameAndAlias[1];

                    if (fieldAlis != null)
                    {
                        CheckName(fieldAlis);
                    }

                    fieldObj.FunExpression = fieldName;
                    fieldObj.Alias = fieldAlis;
                    fields.Add(fieldObj);


                }
                else
                {
                    var fieldArray = fieldPart.SplitString();
                    foreach (var field in fieldArray)
                    {
                        var fieldObj = new Field();
                        var nameAndAlias = GetFieldAndAlias(field.Trim());

                        var fieldName = nameAndAlias[0];
                        var fieldAlis = nameAndAlias[1];

                        CheckName(fieldName);

                        if (fieldAlis != null)
                        {
                            CheckName(fieldAlis);

                        }

                        fieldObj.Name = fieldName;
                        fieldObj.Alias = fieldAlis;
                        fields.Add(fieldObj);
                    }


                }


            }

            return fields;

        }

        private static Dictionary<string, string> GetOrderBys(string orderStr)
        {

            var dic = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            if (orderStr.IsEmpty()) return dic;

            foreach (var field in orderStr.Split(','))
            {
                var field2 = field.Trim();
                if (field2.IsEmpty()) continue;

                if (field2.EndsWith('+'))
                {
                    field2 = field2.Substring(0, field2.Length - 1).Trim();
                    CheckName(field2);
                    dic[field2] = "asc";
                }
                else if (field2.EndsWith('-'))
                {
                    field2 = field2.Substring(0, field2.Length - 1).Trim();
                    CheckName(field2);
                    dic[field2] = "desc";
                }

            }


            return dic;

        }

        private static Condition GetFunCondition(string funStr)
        {


            var rBracketsIndex = funStr.LastIndexOf(')');
            if (rBracketsIndex + 1 == funStr.Length) return null;

            var rPart = funStr.Substring(rBracketsIndex + 1).TrimStart();

            var oper = SqlOperators.FirstOrDefault(rPart.StartsWith);
            if (oper != null)
            {
                var cdt = new Condition();
                cdt.Operator = oper;
                cdt.Value = rPart.Substring(oper.Length);

                var funExp = funStr.Substring(0, rBracketsIndex + 1).Trim();
                cdt.FunExpression = funExp;

                return cdt;
            }

            return null;
        }



        private static List<AssociatedTable> GetAssociatedTables(JsonProperty prop)
        {

            var assTbs = new List<AssociatedTable>();
            var key = prop.Name;

            var field = key.Substring(0, key.Length - 1);
            if (field.IsEmpty()) return assTbs;
            var value = prop.Value.ToString().Trim();
            if (value.IsEmpty()) return assTbs;

            string tag = value;
            Condition condition = null;


            if (value.IsSqlFun())
            {

                tag = GetRelatedExp(tag) ?? throw new JsonException("关联信息错误"); ;
                condition = GetFunCondition(value);

            }


            var parts = tag.Split('/');
            if (parts.Length == 2 || parts.Length == 3)
            {

                var associated = new AssociatedTable();
                associated.Tag = tag;
                CheckName(field);

                associated.Field = field;

                if (parts.Length == 3)
                {
                    var pKey = parts[0];
                    if (pKey.IsEmpty()) pKey = "/";
                    associated.ParentKey = pKey;
                    CheckName(parts[1]);
                    CheckName(parts[2]);
                    associated.Table = parts[1];
                    associated.AssociatedField = parts[2];
                }
                else
                {
                    CheckName(parts[0]);
                    CheckName(parts[1]);
                    associated.Table = parts[0];
                    associated.AssociatedField = parts[1];
                }
                associated.Condition = condition;
                assTbs.Add(associated);
            }


            return assTbs;

        }

        private static string GetRealField(string field)
        {

            if (field.Contains(':'))
            {

                field = field.Split(':')[0];

            }

            return field;
        }


        private static JsonCondition GetCommonCondition(JsonProperty prop)
        {

            var key = prop.Name;
            var jsonCdt = new JsonCondition
            {
                Tag = key
            };
            var field = key.Substring(0, key.Length - 2);
            var propValue = prop.Value;

            if (propValue.ValueKind== JsonValueKind.Array)
            {

                var cdt = new Condition
                {
                    Operator = SqlKeys.IN
                };
                if (field.EndsWith(JsonKeys.NOT))
                {
                    field = field.Substring(0, field.Length - 1);
                    cdt.Operator = SqlKeys.NOTIN;
                }

                field = GetRealField(field);

                CheckName(field);

                var values = new List<object>();

                foreach (var v in propValue.EnumerateArray())
                {
                    values.Add(v.GetValue());
                }
                cdt.Value = values;
                cdt.Field = field;
                jsonCdt.Conditions.Add(cdt);

            }
            else
            {
                var cdtCtn = new ConditionCollection();

                LogicType logicType = LogicType.Or;

                if (field.EndsWith(JsonKeys.AND))
                {
                    logicType = LogicType.And;
                    field = field.Substring(0, field.Length - 1);
                }
                else if (field.EndsWith(JsonKeys.OR))
                {
                    field = field.Substring(0, field.Length - 1);
                }

                field = GetRealField(field);

                CheckName(field);

                bool find = false;

              
                if (propValue.ValueKind == JsonValueKind.String)
                {
                    var strValue = propValue.GetString();
                    var values = strValue.SplitString(";");

                    var index = 0;

                    foreach (var value in values)
                    {
                        var value2 = value.TrimStart();
                        if (value2.IsSqlFun())
                        {

                            var cdt = GetFunCondition(value2);
                            if (cdt != null)
                            {
                                cdt.Type = index == 0 ? LogicType.And : logicType;
                                find = true;
                                cdtCtn.Conditions.Add(cdt);
                                index++;
                            }

                        }
                        else
                        {

                            var values2 = value2.SplitString();
                            foreach (var cdtStr in values2)
                            {
                                var cdtStr2 = cdtStr.TrimStart();
                                var oper = SqlOperators.FirstOrDefault(m => cdtStr2.StartsWith(m));
                                if (oper != null)
                                {
                                    var cdt = new Condition();
                                    cdt.Type = index == 0 ? LogicType.And : logicType;
                                    cdt.Operator = oper;
                                    cdt.Value = cdtStr2.Substring(oper.Length);
                                    cdt.Field = field;
                                    find = true;
                                    cdtCtn.Conditions.Add(cdt);
                                    index++;
                                }

                            }
                        }



                    }

                }

                if (!find)
                {
                    var cdt = new Condition
                    {
                        Operator = "=",
                        Value = propValue.GetValue(),
                        Field = field,
                    };
                    jsonCdt.Conditions.Add(cdt);
                }
                else
                {
                    jsonCdt.Conditions.Add(cdtCtn);
                }

            }



            return jsonCdt;

        }


        private static JsonCondition GetLikeCondition(JsonProperty prop)
        {

            var key = prop.Name;
            var jsonCdt = new JsonCondition
            {
                Tag = key
            };

            var field = key.Substring(0, key.Length - 1);
            field = GetRealField(field);
            CheckName(field);
            var cdt = new Condition
            {
                Operator = SqlKeys.LIKE,
                Value = prop.Value.ToString(),
                Field = field
            };

            jsonCdt.Conditions.Add(cdt);

            return jsonCdt;

        }

        private static StatementType? GetStatementType(JsonProperty prop)
        {

            var value = prop.Value.ToString();
            if (value == "select")
                return StatementType.Select;
            if (value == "update")
                return StatementType.Update;
            if (value == "delete")
                return StatementType.Delete;

            if (value == "insert")
                return StatementType.Insert;

            return null;



        }


        private static JsonCondition GetHavingCondition(JsonProperty prop)
        {

            var key = prop.Name;
            var jsonCdt = new JsonCondition
            {
                Tag = key,
                IsWhere = false
            };

            var values = prop.Value.ToString().SplitString(";");
            var cdtCtn = new ConditionCollection();
            foreach (var value in values)
            {
                var trimedValue = value.TrimStart();
                if (trimedValue.IsSqlFun())
                {
                    var cdt = GetFunCondition(trimedValue);
                    if (cdt != null)
                    {
                        cdtCtn.Conditions.Add(cdt);

                    }
                }

            }

            if (cdtCtn.Conditions.Count > 0)
                jsonCdt.Conditions.Add(cdtCtn);

            return jsonCdt;

        }


        private static JsonCondition GetBetweenCondition(JsonProperty prop)
        {

            var key = prop.Name;
            var jsonCdt = new JsonCondition
            {
                Tag = key
            };

            var field = key.Substring(0, key.Length - 1);
            CheckName(field);
            var values = new List<string>();

            var propValue = prop.Value;

            if (propValue.ValueKind== JsonValueKind.Array)
            {
                foreach (var v in propValue.EnumerateArray())
                    values.Add(v.ToString());
            }
            else
            {
                values.Add(propValue.ToString());
            }

            int index = 0;
            foreach (var value in values)
            {

                var cdtCtn = new ConditionCollection();
                var valueArray = value.SplitString();
                if (valueArray.Length != 2) continue;

                var cdt = new Condition
                {
                    Operator = ">=",
                    Value = valueArray[0].Trim(),
                    Field = field

                };
                if (index != 0)
                    cdt.Type = LogicType.Or;

                cdtCtn.Conditions.Add(cdt);

                cdt = new Condition
                {
                    Operator = "<=",
                    Value = valueArray[1].Trim(),
                    Field = field
                };

                cdtCtn.Conditions.Add(cdt);
                jsonCdt.Conditions.Add(cdtCtn);

                index++;

            }

            return jsonCdt;

        }

        private static string GetRelatedExp(string str)
        {
            var lrBracketsIndex = str.IndexOf('(');
            var rBracketsIndex = str.LastIndexOf(')');
            var args = str.Substring(lrBracketsIndex + 1, rBracketsIndex - lrBracketsIndex - 1);
            var argsArray = args.Split(',');
            foreach (var arg in argsArray)
            {
                var arg2 = arg.TrimStart();
                if (arg2[0] != '\'' && arg2[0] != '"' && arg2.Contains('/'))
                {
                    return arg2;

                }
            }

            return null;

        }

        private static JoinInfo GetJoinInfo(JsonProperty prop)
        {

            string field = prop.Name;

            var joinInfo = new JoinInfo
            {
                Type = JoinType.InnerJoin
            };

            if (field.EndsWith(JsonKeys.LEFTJOIN))
            {
                joinInfo.Type = JoinType.LeftJoin;
                field = field.Substring(0, field.Length - JsonKeys.LEFTJOIN.Length);
            }
            else
            {
                field = field.Substring(0, field.Length - JsonKeys.JOIN.Length);

            }

            CheckName(field);

            joinInfo.Field = field;

            var value = prop.Value.ToString();
            if (value.IsSqlFun())
            {
                joinInfo.Condition = GetFunCondition(value);
                value = GetRelatedExp(value);
                joinInfo.Tag = value ?? throw new JsonException("关联信息错误");
            }

            var parts = value.SplitString("/");
            if (parts.Length == 2)
            {
                CheckName(parts[0]);
                CheckName(parts[1]);
                joinInfo.Table = parts[0];
                joinInfo.JoinedField = parts[1];
                return joinInfo;
            }

            return null;


        }



        private JsonTable GetTable(JsonProperty tableProp, string parentTag, int? page, int? count)
        {
            var tableName = tableProp.Name;

            if (!IsTable(tableName)) return null;

            if (tableProp.Value.ValueKind != JsonValueKind.Object) return null;

            var table = new JsonTable
            {
                SelfKey = tableName
            };

            var tableNames = tableName.SplitString(":");
            if (tableNames.Length == 2)
            {
                tableName = tableNames[0];
                table.SelfKey = tableName;
                table.TableAlias = tableNames[1];

            }

            var tb = tableProp.Value;

            if (tableName.EndsWith(JsonKeys.ARRAY))
            {
                tableName = tableName.Substring(0, tableName.Length - 2);
                var count2 = GetCount(tb);
                var page2 = GetPage(tb);

                if (page2.HasValue) page = page2;
                if (count2.HasValue) count = count2;

                table.Query = GetQuery(tb);

                tb = tb.SafeGetProperty(tableName);

                if (tb.ValueKind != JsonValueKind.Object)
                    throw new JsonException($"必须要有{tableName}属性");


            }

            var tbProps = tb.EnumerateObject();
            while (tbProps.MoveNext())
            {

                var current = tbProps.Current;

                var key = current.Name;

                var value = current.Value.IsNull() ? null : current.Value.ToString();

                if (key == JsonKeys.COLUMN)
                {
                    table.Fields = GetFields(value, false);
                }
                else if (key == JsonKeys.GROUP)
                {
                    table.GropuBys = GetFields(value, true);
                }
                else if (key == JsonKeys.ORDER)
                {
                    table.OrderBys = GetOrderBys(value);
                }
                else if (key == JsonKeys.COMBINE)
                {
                    table.Combine = value;
                }
                else if (key.EndsWith(JsonKeys.RELATED))
                {
                    table.AssociatedTables = GetAssociatedTables(current);
                }
                else if (key.EndsWith(JsonKeys.CONDITION))
                {
                    table.Conditions.Add(GetCommonCondition(current));
                }
                else if (key.EndsWith(JsonKeys.LIKE))
                {
                    table.Conditions.Add(GetLikeCondition(current));
                }
                else if (key == JsonKeys.HAVING)
                {
                    table.Conditions.Add(GetHavingCondition(current));
                }
                else if (key.EndsWith(JsonKeys.BETWEEN))
                {
                    table.Conditions.Add(GetBetweenCondition(current));
                }
                else if (key.EndsWith(JsonKeys.JOIN))
                {
                    var joinInfo = GetJoinInfo(current);
                    if (joinInfo != null) table.Joins.Add(joinInfo);
                }
                else if (key == JsonKeys.STATEMENT)
                {
                    table.Statement = GetStatementType(current);
                }
                else
                {
                    string op = null;
                    op = ValueOperators.FirstOrDefault(m => key.EndsWith(m));
                    if (op != null)
                    {
                        key = key.Substring(0, key.Length - op.Length);

                    }
                    CheckName(key);

                    table.FieldValues.Add(new FieldValue { Name = key, Value = current.Value.GetValue(), Operator = op ?? "=" });

                }
            }

            CheckName(tableName);
            table.Table = tableName;
            table.Page = page;
            table.Count = count;
            table.ParentKey = parentTag;

            return table;

        }

        public List<JsonTable> Parse(string json)
        {
            try
            {

                var jsonEle = JsonSerializer.Deserialize<JsonElement>(json);
                var eleList = jsonEle.EnumerateObject();

                while (eleList.MoveNext())
                {
                    var current = eleList.Current;
                    var key = current.Name;
                    var value = jsonEle.GetProperty(key);                
                    var valueKind = value.ValueKind;

                    if (valueKind == JsonValueKind.Null) continue;

                    if (key == JsonKeys.ARRAY)
                    {

                        if (valueKind == JsonValueKind.Object)
                        {
                            var page = GetPage(value);
                            var count = GetCount(value);
                            var query = GetQuery(value);
                            var tableEles = value.EnumerateObject();
                            while (tableEles.MoveNext())
                            {
                                var currentTable= tableEles.Current;
                                if (currentTable.Value.ValueKind == JsonValueKind.Object)
                                {
                                    var tb = GetTable(currentTable, key, page, count);
                                    if (tb != null)
                                    {
                                        tables.Add(tb);
                                        tb.Query = query;
                                    }

                                }
                            }
                          
                        }
                        else if (valueKind == JsonValueKind.Array) //批量插入
                        {
                            var array = value.EnumerateArray();
                            foreach (var ele in array)
                            {
                                if (ele.ValueKind != JsonValueKind.Object) continue;

                                var tableEles = ele.EnumerateObject();

                                while (tableEles.MoveNext())
                                {
                                    var currentTable = tableEles.Current;
                                    if (currentTable.Value.ValueKind == JsonValueKind.Object)
                                    {
                                        var tb = GetTable(currentTable, key, null, null);
                                        if (tb != null)
                                        {
                                            tables.Add(tb);

                                        }

                                    }
                                }

                            }
                        }

                    }
                    else
                    {
                        var tb = GetTable(current, null, null, null);
                        if (tb != null) tables.Add(tb);

                    }

                }
            }
            catch (Exception ex)
            {
                throw new JsonException(ex.Message);
            }

            return tables;

        }

    }
}
