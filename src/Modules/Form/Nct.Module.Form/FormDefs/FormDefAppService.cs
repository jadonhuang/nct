using Microsoft.AspNetCore.Mvc;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.OpenXml;
using Nct.Application.Form.RuleDefs;
using Nct.DataAccess.DynamicQuery;
using Nct.DataAccess.Json;
using Nct.LinqQuery;
using Nct.Module.Core.DicValues;
using Nct.Module.Core.Organizations;
using Nct.Module.Core.Users;
using Nct.Module.Form.DataSources;
using Nct.Module.Form.DataSources.Dtos;
using Nct.Module.Form.DbTables;
using Nct.Module.Form.DbTables.Dtos;
using Nct.Module.Form.FormDefs.Dtos;
using Nct.Module.Form.RuleDefs;
using Nct.ObjectMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using StatementType = Nct.DataAccess.Json.StatementType;

namespace Nct.Module.Form.FormDefs
{


    public class FormDefAppService : CrudAppService<FormDef, FormDefDto, FormDefDto, CreateFormDefDto>, IFormDefAppService
    {

        public const string IdField = "Id";
        private const string CreateTimeField = "CreateTime";
        private const string CacheKeyPrefix = "formdef_";
        private const string RefField = "__RefId";

        private const string ValueKey = "Value";
        private const string TextKey = "Text";

        private readonly IDbTableAppService _dbTableAppService;
        private readonly IResultBuilder _resultBuilder;
        private readonly IDataSourceAppService _dataSourceAppService;
        private readonly ISqlJsonParser _sqlJsonParser;
        private readonly IDicValueAppService _dicValueAppService;
        private readonly IRuleDefAppService _ruleDefAppService;
        private readonly IOrganizationAppService _orgAppService;
        private readonly IUserAppService _userAppService;

        private readonly Dictionary<string, string[]> cacheDsData;
        private readonly Dictionary<string, object> cacheDsAllData;
        private readonly Dictionary<string, object> systemFieldValues;
        private readonly Dictionary<string, List<FieldInfo>> cacheTableFields;

        public FormDefAppService(IRepository<FormDef> repository, IDbTableAppService dbTableAppService,
            IResultBuilder resultBuilder,
            IDataSourceAppService dataSourceAppService, ISqlJsonParser sqlJsonParser,
            IDicValueAppService dicValueAppService, IRuleDefAppService ruleDefAppService, IOrganizationAppService orgAppService,
            IUserAppService userAppService


            ) : base(repository)
        {

            _dbTableAppService = dbTableAppService;
            _resultBuilder = resultBuilder;
            _dataSourceAppService = dataSourceAppService;
            _sqlJsonParser = sqlJsonParser;
            _dicValueAppService = dicValueAppService;
            cacheDsData = [];
            cacheDsAllData = [];
            systemFieldValues = [];
            cacheTableFields = [];
            _ruleDefAppService = ruleDefAppService;
            _orgAppService = orgAppService;
            _userAppService = userAppService;


        }

        protected override void SetPermissionNames()
        {
            CreatePermissionName = "addformdef";
            UpdatePermissionName = "editformdef";
            DeletePermissionName = "delformdef";

        }


        public async Task<List<FormDefDto>> GetAllList()
        {

            var list = await Repository.GetAllListAsync();
            var dtos = list.MapTo<FormDefDto>();
            return dtos;

        }

        private void CheckCodeExist(CreateFormDefDto dto)
        {
            var model = Repository.GetAll().Where(m => m.Code == dto.Code && m.Id != dto.Id).FirstOrDefault();
            if (model != null)
                throw new BusinessException("编码已经存在");


        }

        private void CheckServerRule(CreateFormDefDto dto)
        {
            if (dto.ServerRule.IsEmpty()) return;

            _ruleDefAppService.TryCompileFormRuleClassCode(dto.ServerRule);

        }

        protected override IQueryable<FormDef> CreateQuery(FormDefDto dto)
        {
            return base.CreateQuery(dto).WhereIf(m => m.Name.Contains(dto.Name), dto.Name.IsNotEmpty())
            .WhereIf(m => m.Code.Contains(dto.Code), dto.Code.IsNotEmpty())
                .WhereIf(m => m.Descpt.Contains(dto.Descpt), dto.Descpt.IsNotEmpty());
        }


        private static void SetCreateDbTableDtoFields(CreateDbTableDto dto, List<BaseItem> items)
        {
            foreach (var item in items)
            {

                var dbField = new DbFieldDto
                {
                    Name = item.Name,
                    Remark = item.Title,
                };
                if (item is Text text)
                {
                    dbField.FieldType = "str";
                    dbField.Remark = item.Title;
                    if (text.MaxLen == 0 || text.MaxLen > 4000)
                    {
                        dbField.FieldType = "text";
                    }
                    else
                    {
                        dbField.Length = text.MaxLen;

                    }

                }
                else if (item is RichText)
                {
                    dbField.FieldType = "text";
                }
                else if (item is Image)
                {
                    dbField.FieldType = "img";
                }
                else if (item is Upload)
                {
                    dbField.FieldType = "file";
                }

                else if (item is DataSourceItem)
                {
                    dbField.FieldType = "str";
                    dbField.Length = 2000;
                }
                else if (item is Number number)
                {
                    if (number.Precision == 2)
                        dbField.FieldType = "money";
                    else if (number.Precision == 0)
                        dbField.FieldType = "int";
                    else
                        dbField.FieldType = "float";

                }
                else if (item is DatePicker)
                {
                    dbField.FieldType = "datetime";

                }
                else if (item is Location)
                {
                    dbField.FieldType = "str";
                    dbField.Length = 200;
                }

                if (dbField.FieldType != null)
                    dto.Fields.Add(dbField);


            }

        }

        private async Task<(List<DbTable> dbTables, Dictionary<string, CreateDbTableDto> itemTables)> CreateDbTable(List<BaseItem> items, CreateFormDefDto dto)
        {


            var tableDtos = new List<CreateDbTableDto>();

            var createDbTableDto = new CreateDbTableDto
            {
                Fields = new List<DbFieldDto>(),
                Descpt = dto.Name,
                Name = dto.Code
            };

            SetCreateDbTableDtoFields(createDbTableDto, items);
            tableDtos.Add(createDbTableDto);

            var itemTables = new Dictionary<string, CreateDbTableDto>();

            var tableItems = items.Where(m => m is Table).Select(m => (Table)m);
            foreach (var table in tableItems)
            {
                createDbTableDto = new CreateDbTableDto
                {
                    Fields = new List<DbFieldDto>(),
                    Descpt = table.Title,
                    Name = table.Name
                };
                SetCreateDbTableDtoFields(createDbTableDto, table.Items);
                var refField = new DbFieldDto { FieldType = "str", Length = 50, Name = RefField };
                createDbTableDto.Fields.Add(refField);
                tableDtos.Add(createDbTableDto);
                itemTables[table.Id] = createDbTableDto;
            }


            var tables = new List<DbTable>();
            var tbIndex = 0;
            foreach (var tb in tableDtos)
            {
                var index = 1;
                while (true)
                {
                    try
                    {
                        await _dbTableAppService.Create(tb);
                        tables.Add(new DbTable { Name = tb.Name, RefField = tbIndex == 0 ? null : RefField });
                        break;

                    }
                    catch (BusinessException ex)
                    {
                        if (ex.Message.Contains("已存在"))
                        {
                            index++;
                            tb.Name += index;

                        }
                        else
                        {
                            throw;
                        }
                    }
                }

                tbIndex++;

            }

            return (tables, itemTables);

        }



        public override async Task<FormDefDto> Create(CreateFormDefDto dto)
        {

            CheckCodeExist(dto);
            CheckServerRule(dto);
            var result = await base.Create(dto);

            if (dto.AutoCreateTable != 1)
                return result;

            var design = dto.Data.ToObject<DesignInfo>();
            var items = design.AllItems;

            if (items.Count == 0) return result;

            var (dbTables, itemTables) = await CreateDbTable(items, dto);

            var mainTable = dbTables[0].Name;

            var jNode = JsonNode.Parse(dto.Data).AsObject();
            jNode["DbTables"] = JsonNode.Parse(dbTables.ToJson());

            var jsonItems = (JsonArray)jNode["Items"];

            foreach (JsonObject item in jsonItems.Cast<JsonObject>())
            {

                var type = item["Type"].ToString();
                if (type != "Table")
                {
                    if (!item.TryGetPropertyValue("Field", out _)) continue;
                    item["Table"] = mainTable;
                    item["Field"] = item["Name"].ToString();
                }
                else
                {
                    var id = item["Id"].ToString();
                    var tbName = itemTables[id].Name;
                    item["Table"] = tbName;
                    var subItems = (JsonArray)item["Items"];
                    foreach (JsonObject subItem in subItems.Cast<JsonObject>())
                    {
                        if (!subItem.TryGetPropertyValue("Field", out _)) continue;
                        subItem["Table"] = tbName;
                        subItem["Field"] = subItem["Name"].ToString();

                    }


                }

            }
            result.Code = jNode.ToJson();
            Repository.GetAll().Where(m => m.Id == result.Id).Update(m => new FormDef { Data = result.Code });
            return result;
        }


        public override async Task<FormDefDto> Update(CreateFormDefDto dto)
        {

            var model = await Get(dto.Id);
            if (model == null) return null;
            dto.Code = model.Code;
            CheckServerRule(dto);
            var result = await base.Update(dto);

            RemoveFormDefCache(result.Code);
            return result;
        }

        private void RemoveFormDefCache(string code)
        {

            var key = CacheKeyPrefix + code.ToLower();
            Cache.Remove(key);
        }

        public FormDef GetByCode(string code)
        {
            Check.NotNullOrEmpty(code, nameof(code));
            var key = CacheKeyPrefix + code.ToLower();
            return Cache.Get(key, () =>
             {
                 return Repository.GetAll().Where(m => m.Code == code).FirstOrDefault();

             });

        }



        public override async Task<int> Delete(int id)
        {
            var form = await Get(id);
            if (form != null)
            {
                int num = await base.Delete(id);
                RemoveFormDefCache(form.Code);
                return num;
            }

            return 0;
        }


        private void InitTableFields(string table)
        {
            if (!cacheTableFields.ContainsKey(table))
                cacheTableFields[table] = _dbTableAppService.GetDbFields(table);
        }

        private async Task InitSysFieldsValue()
        {
            foreach (var kv in Consts.SystemFields)
            {
                var field = kv.Key;
                if (field == IdField) continue;

                if (field == "CreateTime")
                    systemFieldValues[field] = DateTime.Now;

                else if (field == "CreatorId")
                    systemFieldValues[field] = Session.UserId;

                else if (field == "OrgId")
                    systemFieldValues[field] = await _userAppService.GetUserOrgId(Session.UserId.Value);

            }

        }

        private void SetSysFieldsValue(Dictionary<string, object> row, string table)
        {

            var fields = cacheTableFields[table];

            foreach (var kv in systemFieldValues)
            {
                var field = kv.Key;

                if (!fields.Any(m => m.Name.IgnoreCaseEqual(field))) continue;

                var value = kv.Value;
                if (value is DateTime time)
                {
                    value = time.AddMilliseconds(1);
                    systemFieldValues[field] = value;

                }

                row[field] = value;

            }



        }

        private static string GetIdValue()
        {
            return Guid.NewGuid().ToString();
        }


        private async Task<string[]> ConvertItemTextOrValue(string value, DataSourceItem item, bool isToText)
        {

            if (value.IsEmpty()) return null;

            if (item is Switch switchItem)
            {

                var activeText = switchItem.ActiveText.IsEmpty() ? "是" : switchItem.ActiveText;
                var inactiveText = switchItem.InActiveText.IsEmpty() ? "否" : switchItem.InActiveText;

                if (isToText)
                {

                    if (value == "1") return [activeText];
                    return [inactiveText];

                }
                else
                {
                    if (value == activeText)
                        return ["1"];
                    return ["0"];

                }

            }


            var key = value + item.DataSourceCode;
            if (cacheDsData.TryGetValue(key, out string[] values))
            {
                return values;

            }
            var valArray = value.Split(',');
            var dsCode = item.DataSourceCode + "__" + item.DataSourceType;
            if (item.DataSourceType == 1)
            {
                var dicValues = await _dicValueAppService.GetValuesByCode(item.DataSourceCode);
                string[] texts;
                if (isToText)
                    texts = dicValues.Where(m => valArray.Contains(m.Value)).Select(m => m.Name).ToArray();
                else
                    texts = dicValues.Where(m => valArray.Contains(m.Name)).Select(m => m.Value).ToArray();
                cacheDsData[key] = texts;
                return texts;
            }
            else if (item.DataSourceType == 2)
            {
                DataTable dt = (DataTable)cacheDsAllData.GetOrDefault(dsCode);
                if (dt == null)
                {
                    dt = _dataSourceAppService.GetDataTable(new DsDataQueryDto { Code = item.DataSourceCode });
                    cacheDsAllData[dsCode] = dt;
                }

                var valueField = isToText ? item.ValueField : item.TextField;
                var textField = isToText ? item.TextField : item.ValueField;

                var texts = dt.AsEnumerable().Where(m => valArray.Contains(m[valueField].ToString())).Select(m => m[textField].ToString()).ToArray();
                cacheDsData[key] = texts;
                return texts;

            }
            else if (item.DataSourceType == 3)
            {

                var valueField = isToText ? item.ValueField : item.TextField;
                var textField = isToText ? item.TextField : item.ValueField;

                var data = cacheDsAllData.GetOrDefault(dsCode);
                if (data == null)
                {
                    var queryJson = new { page = 1, count = 1000, Table = new { } }.ToJson();
                    data = ((PageTResult<object>)(await GetFormPageData(item.DataSourceCode, queryJson, false)).Data).Rows;
                    cacheDsAllData[dsCode] = data;
                }

                if (data is DataTable dt)
                {
                    var texts = dt.AsEnumerable().Where(m => valArray.Contains(m[valueField].ToString())).Select(m => m[textField].ToString()).ToArray();
                    cacheDsData[key] = texts;
                    return texts;
                }

                if (data is List<dynamic> list)
                {

                    var txtList = new List<string>();
                    foreach (var row in list)
                    {
                        var dicRow = row as IDictionary<string, object>;
                        var rowValue = dicRow.GetOrDefault(valueField)?.ToString();
                        if (rowValue.IsEmpty()) continue;
                        if (valArray.Contains(rowValue))
                        {
                            txtList.Add(dicRow.GetOrDefault(textField)?.ToString());
                            break;
                        }

                    }

                    var texts = txtList.ToArray();
                    cacheDsData[key] = texts;
                    return texts;
                }



            }

            return null;
        }



        private async Task ResolveDataSourceItemValueText(object rows, List<DataSourceItem> items, bool isNameValue = false)
        {

            if (rows is DataTable table)
            {

                foreach (var item in items)
                {

                    var field = isNameValue ? item.Name : item.Field;

                    var fieldObj = field + "$$";

                    if (!table.Columns.Contains(fieldObj))
                    {
                        table.Columns.Add(fieldObj, typeof(object));
                    }

                    foreach (DataRow row in table.Rows)
                    {
                        var value = row[field].ToString();
                        var texts = await ConvertItemTextOrValue(value, item, true);
                        var val = BuildObjectItemData(row[field], texts, item);
                        row[fieldObj] = val;

                    }

                    table.Columns.Remove(field);
                    table.Columns[fieldObj].ColumnName = field;

                }



            }
            else if (rows is IEnumerable<object> list)
            {

                foreach (var item in items)
                {
                    var field = isNameValue ? item.Name : item.Field;

                    foreach (var row in list)
                    {

                        if (row is IDictionary<string, object> dicRow)
                        {
                            var value = dicRow[field]?.ToString();
                            var texts = await ConvertItemTextOrValue(value, item, true);
                            dicRow[field] = BuildObjectItemData(dicRow[field], texts, item);
                        }
                    }
                }

            }


        }

        private void AddSystemField(List<string> fields, string table)
        {
            var dbFields = cacheTableFields[table];
            foreach (var key in Consts.SystemFields.Keys)
            {
                if (fields.Any(m => m.IgnoreCaseEqual(key))) continue;
                if (!dbFields.Any(m => m.Name.IgnoreCaseEqual(key))) continue;

                fields.Insert(0, key);
            }
        }


        [RemoteService(false)]

        public DesignInfo GetDesignInfoByCode(string formCode, out FormDef formDef)
        {
            formDef = GetByCode(formCode);
            if (formDef == null)
                throw new BusinessException("表单不存在");

            return formDef.Data.ToObject<DesignInfo>();
        }


        private async Task<string> GetDataAuthWhere(int dataAuth)
        {

            string where = null;
            if (dataAuth == 1)
            {
                //自己所属机构
                where = $"(OrgId is null or OrgId in (select a.OrgId from Core_UserOrganization a where a.UserId={Session.UserId}))";
            }
            else if (dataAuth == 2)
            {
                //自己所属机构及其下级
                var orgIds = await _orgAppService.GetUserOrgAndChildrenIds(Session.UserId.Value);
                if (orgIds.Count > 0)
                {
                    var orgIdStr = string.Join(",", orgIds);
                    where = $"(OrgId is null or OrgId in ({orgIdStr}))";
                }
                else
                {
                    where = "1>1";
                }
            }
            else if (dataAuth == 3)
            {
                //仅自己
                where = $"CreatorId = {Session.UserId}";
            }


            return where;

        }



        private static string GetSummaryCols(List<ListCol> cols)
        {

            var sb = new StringBuilder();

            foreach (var col in cols)
            {

                var field = col.Field;
                var type = col.SummaryType;
                if (field.IsEmpty() || type.IsEmpty()) continue;

                switch (type)
                {
                    case "sum":
                    case "avg":
                        sb.Append($"{type}(case when ISNUMERIC({field})=1 then {field} else 0 end) {field},");
                        break;
                    case "max":
                    case "min":
                        sb.Append($"{type}({field}) {field},");
                        break;

                }

            }

            if (sb.Length > 0)
            {
                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();

        }



        [RemoteService(false)]
        public async Task<RetResult> GetFormPageData(string formCode, string json, bool resolveValueText)
        {

            var designInfo = GetDesignInfoByCode(formCode, out _);

            const string TableKey = "Table";

            var pageResult = new PageTResult<object>();

            var jNode = JsonNode.Parse(json).AsObject();

            var listCols = designInfo.ListConfig.List.Cols;

            var dataAuthen = designInfo.ListConfig.DataAuth;
            if (dataAuthen > 0)
            {
                var authenWhere = await GetDataAuthWhere(dataAuthen);
                SqlHelper.SetContextExtraWhere(authenWhere);
            }

            var table = designInfo.DbTables[0].Name;
            var isNormalSave = designInfo.DataSaveType != 2;
            var listCfg = designInfo.ListConfig;
            List<string> fields = null;
            string fieldsStr = null;
            if (isNormalSave)
            {
                fields = listCfg.List.Cols.Where(m => m.Field.IsNotEmpty()).Select(m => $"{m.Field}").ToList();
                InitTableFields(table);
                AddSystemField(fields, table);

                fieldsStr = string.Join(",", fields);
                CallContext.SetData("_fields", fieldsStr);
            }


            var hasSummary = listCfg.List.Summary != null && listCfg.List.Summary.Enabled && isNormalSave;

            if (designInfo.ListConfig.DataSource.Enabled)
            {

                var code = listCfg.DataSource.Code;
                var dto = new DsDataPageQueryDto
                {
                    Code = code,
                    PageIndex = jNode["page"].AsValue().GetValue<int>(),
                    PageSize = jNode["count"].AsValue().GetValue<int>()
                };
                var query = jNode[TableKey].ToJsonString();
                dto.Query = query;
                var result = _dataSourceAppService.GetDataTablePage(dto);
                pageResult.Total = result.Total;
                pageResult.Rows = result.Rows;

                if (hasSummary)
                {
                    CallContext.SetData(Consts.SummaryFields, GetSummaryCols(listCfg.List.Cols));
                    var dsQuery = dto as DsDataQueryDto;
                    var dt = _dataSourceAppService.GetDataTable(dsQuery);
                    pageResult.Data = dt;

                }

            }
            else
            {
                var items = designInfo.AllItems;
                table = char.ToUpper(table[0]) + table[1..];
                if (!isNormalSave)
                {
                    fieldsStr = "Id,Value";
                }

                jNode["query"] = 2;
                var tbNode = jNode[TableKey];
                tbNode["@column"] = fieldsStr;

                jNode.Remove(TableKey);
                jNode.Add(table, tbNode);

                json = $"{{\"{table}[]:rows\":{jNode.ToJsonString()}}}";

                var dfRltBld = (DefaultResultBuilder)_resultBuilder;
                dfRltBld.SqlJsonPrivacyChecker = NullSqlJsonPrivacyChecker.Instance;

                var data = await dfRltBld.Build(json, StatementType.Select);
                var rows = data["rows"];
                var total = (int)data["total"];

                pageResult.Total = total;
                pageResult.Rows = rows;

                if (hasSummary)
                {

                    var query = "{\"Tb\":" + jNode[table].ToJsonString() + "}";
                    var jsonTable = _sqlJsonParser.Parse(query).First();
                    var cdtInfo = new DataSourceJsonSqlCreater().GetConditionInfo(jsonTable);
                    var summaryCols = GetSummaryCols(listCfg.List.Cols);
                    string sql = $"select {summaryCols} from {table} t {cdtInfo.Condition}";
                    var dt = Db.QueryTable(sql, cdtInfo.Parameters);
                    pageResult.Data = dt;

                }

            }

            if (resolveValueText)
            {
                var dsItems = designInfo.AllItems.Where(m => m is DataSourceItem && listCols.Any(n => m.Field == n.Field)).Select(m => (DataSourceItem)m).ToList();
                if (dsItems.Count > 0)
                {
                    await ResolveDataSourceItemValueText(pageResult.Rows, dsItems);
                }
            }

            pageResult.PageIndex = 1;
            pageResult.PageSize = 10;
            return Success(pageResult);

        }




        [Post]
        public async Task<RetResult> GetPageData(PageDataQueryDto dto)
        {

            var result = await GetFormPageData(dto.FormCode, dto.QueryJson, true);

            return result;


        }

        private static Dictionary<string, object> DynamicRowToDicRow(DynamicRow row)
        {
            if (row == null) return null;
            Dictionary<string, object> dic = [];
            foreach (var kv in row)
            {
                dic[kv.Key] = kv.Value;
            }
            return dic;
        }

     
        private static Dictionary<string, object> BuildObjectItemData(object value, object extra, BaseItem item)
        {
            return new Dictionary<string, object>
            {
                [ValueKey] = value,
                [TextKey] = extra,
                ["Type"] = item.Type
            };
        }

        private static void CombineExtraFieldsData(Dictionary<string, object> row, Dictionary<BaseItem, string> extras)
        {
            if (row == null) return;
            foreach (var kv in extras)
            {
                var val = BuildObjectItemData(row[kv.Key.Name], row[kv.Value], kv.Key);
                row.Remove(kv.Value);
                row[kv.Key.Name] = val;

            }

        }

        public async Task<RetResult> GetData(string formCode, string id)
        {

            var designInfo = GetDesignInfoByCode(formCode, out FormDef formDef);

            var items = designInfo.AllItems;

            var mainTable = designInfo.DbTables[0].Name;

            InitTableFields(mainTable);

            var fields = items.Where(m => m.Table.IgnoreCaseEqual(mainTable) && m.Field.IsNotEmpty()).Select(m => $"[{m.Field}] [{m.Name}]").ToList();
            AddSystemField(fields, mainTable);
          
            var fieldsStr = string.Join(",", fields);

            Dictionary<string, object> mainRow = null;

            if (designInfo.DataSaveType != 2)
            {

                var row = Sql.From(mainTable).Where(m => m[IdField] == id).Fields(fieldsStr).FirstOrDefault();
                mainRow = DynamicRowToDicRow(row);
              
            }
            else
            {
                var rows = Sql.From(mainTable).ToList();
                if (rows.Count > 0)
                {
                    mainRow = [];

                    foreach (var item in items)
                    {
                        var row = rows.FirstOrDefault(m => m[IdField] == item.Name);
                        if (row == null) continue;

                        mainRow[item.Name] = row["Value"];

                    }
                    if (mainRow.Keys.Count == 0)
                        mainRow = null;
                }

            }
            if (mainRow == null)
                return Success();


            var dsItems = items.Where(m => m is DataSelect && m.Table.IgnoreCaseEqual(mainTable)).Select(m => (DataSourceItem)m).ToList();

            if (dsItems.Count > 0)
            {
                await ResolveDataSourceItemValueText(new List<dynamic> { mainRow }, dsItems, true);
            }

            if (designInfo.DbTables.Count > 1)
            {

                for (var i = 1; i < designInfo.DbTables.Count; i++)
                {

                    var dbTable = designInfo.DbTables[i];
                    var table = dbTable.Name;
                    var tbItem = (Table)items.Where(m => m.Table.IgnoreCaseEqual(table) && m.IsTable).FirstOrDefault();
                    var refField = dbTable.RefField;
                    if (tbItem != null)
                    {

                        InitTableFields(table);

                        items = tbItem.Items;
                        fields = items.Where(m => m.Field.IsNotEmpty()).Select(m => $"[{m.Field}] [{m.Name}]").ToList();
                        AddSystemField(fields, table);
                       
                        fieldsStr = string.Join(",", fields);

                        var orderField = IdField;
                        if (cacheTableFields[table].Any(m => m.Name.IgnoreCaseEqual(CreateTimeField)))
                            orderField = CreateTimeField;

                        var rows = Sql.From(table).Where(m => m[refField] == id).Fields(fieldsStr).OrderBy(m => orderField).ToList();
                        var dicRows = new List<Dictionary<string, object>>();
                        foreach (var row in rows)
                        {
                            var dicRow = DynamicRowToDicRow(row);
                            dicRows.Add(dicRow);
                        }

                        if (rows.Count > 0)
                        {
                            dsItems = items.Where(m => m is DataSelect).Select(m => (DataSourceItem)m).ToList();
                            if (dsItems.Count > 0)
                            {
                                await ResolveDataSourceItemValueText(dicRows, dsItems, true);
                            }
                        }
                        mainRow[tbItem.Name] = dicRows;

                    }
                    else
                    {

                        var notTbItems = items.Where(m => m.Table.IgnoreCaseEqual(table) && !m.IsTable && m.Field.IsNotEmpty()).ToList();
                        fields = notTbItems.Select(m => $"[{m.Field}] [{m.Name}]").ToList();                      
                        fieldsStr = string.Join(",", fields);

                        var relateRow = Sql.From(table).Where(m => m[refField] == id).Fields(fieldsStr).FirstOrDefault();
                        if (relateRow != null)
                        {
                            var row = DynamicRowToDicRow(relateRow);
                            dsItems = notTbItems.Where(m => m is DataSelect).Select(m => (DataSourceItem)m).ToList();
                            if (dsItems.Count > 0)
                            {
                                await ResolveDataSourceItemValueText(row, dsItems, true);
                            }
                            foreach (var kv in row)
                            {
                                mainRow[kv.Key] = kv.Value;
                            }

                        }
                    }


                }


            }

            return Success(mainRow);

        }

        private static void CheckItemValue(BaseItem item, object value)
        {

            var valueIsNull = value == null || value.ToString().IsEmpty();
            if (valueIsNull && item.Visible && item.Required && item.Field.IsNotEmpty())
            {
                var title = item.Title.IsEmpty() ? item.Name : item.Field;
                throw new BusinessException($"{title} 的值不能为空");

            }

        }

        private static string GetIndentityValue()
        {

            var id = Db.ExecuteScalar<int>("SELECT @@IDENTITY");
            return id.ToString();

        }

        private static string SetIdValue(Dictionary<string, object> row, string idValue = null)
        {

            idValue = idValue.IsEmpty() ? GetIdValue() : idValue;
            row[IdField] = idValue;
            return idValue;
        }

        private bool IdFieldIsIdentity(string tb)
        {

            var dbFields = cacheTableFields[tb];
            var idIsIndentity = dbFields.First(m => m.Name.IgnoreCaseEqual(IdField)).IsIdentity;

            return idIsIndentity;
        }


        private static string GetTableRefField(DesignInfo designInfo, string tb)
        {

            var dbTable = designInfo.DbTables.Find(m => m.Name.IgnoreCaseEqual(tb));
            var refField = dbTable.RefField;

            return refField;
        }


        private static bool GetItemValue(JsonElement json, string name, out object value, string subName = ValueKey)
        {

            json = json.SafeGetProperty(name);

            if (json.ValueKind == JsonValueKind.Object)
            {
                json = json.SafeGetProperty(subName);
            }

            if (json.ValueKind == JsonValueKind.Undefined)
            {
                value = null;
                return false;

            }
            if (json.ValueKind == JsonValueKind.Null)
            {
                value = null;
            }
            else
            {
                value = json.ToString();

            }
            return true;

        }

        private static void SetAndCheckItemValue(Dictionary<string, object> row, JsonElement jsonEle, BaseItem item, string field = null)
        {

            var hasValue = GetItemValue(jsonEle, item.Name, out object val);

            CheckItemValue(item, val);

            if (!hasValue) return;

            field ??= item.Field;
            row[field] = val;


        }

        public async Task<RetResult> SaveData(SaveDataDto dto)
        {

            var designInfo = GetDesignInfoByCode(dto.FormCode, out FormDef formDef);

            var ruleCode = formDef.ServerRule;

            var mainTable = designInfo.DbTables[0].Name;

            var jsonData = dto.Data.ToObject<JsonElement>();

            var tableData = new Dictionary<string, List<Dictionary<string, object>>>();

            var tbItemTables = new Dictionary<string, string>();

            var id = dto.Id;

            var isAdd = true;

            var isNormalSave = designInfo.DataSaveType != 2;

            if (isNormalSave)
            {
                isAdd = dto.Id.IsEmpty() || Sql.From(mainTable).Fields(IdField).Where(m => m[IdField] == id).FirstOrDefault() == null;

            }

            string field;

            foreach (var item in designInfo.AllItems)
            {

                if (item.IsTable)
                {
                    tbItemTables[item.Table] = null;

                    var tbJsonData = jsonData.SafeGetProperty(item.Name);

                    if (tbJsonData.ValueKind == JsonValueKind.Undefined && !item.Required) continue;

                    if (tbJsonData.ValueKind != JsonValueKind.Array)
                    {
                        return Error($"{item.Title} 数据格式错误");
                    }

                    var rows = tbJsonData.EnumerateArray();

                    if (item.Required && !rows.Any())
                    {
                        return Error($"{item.Title} 表格数据不能为空");
                    }
                    if (!rows.Any()) continue;

                    var tableItem = item as Table;

                    if (tableItem.MaxRows > 0 && rows.Count() > tableItem.MaxRows)
                        return Error($"{item.Title} 表格最多只能添加{tableItem.MaxRows}条数据");

                    var dataRows = new List<Dictionary<string, object>>();

                    foreach (var row in rows)
                    {

                        var dataRow = new Dictionary<string, object>();
                        dataRow[IdField] = row.SafeGetString(IdField);
                        foreach (var subItem in tableItem.Items)
                        {
                            field = subItem.Field;
                            if (field.IsEmpty()) continue;

                            SetAndCheckItemValue(dataRow, row, subItem);

                        }
                        dataRows.Add(dataRow);
                    }

                    tableData[item.Table] = dataRows;
                }
                else
                {

                    field = isNormalSave ? item.Field : item.Name;
                    if (field.IsEmpty()) continue;

                    tableData.TryGetValue(item.Table, out List<Dictionary<string, object>> rows);
                    if (rows == null)
                    {
                        rows = [new Dictionary<string, object>()];
                        tableData[item.Table] = rows;
                    }
                    var row = rows[0];

                    SetAndCheckItemValue(row, jsonData, item);


                }

            }

            RuleContext ruleContext = null;
            BaseRule ruleInstance = null;

            if (ruleCode.IsNotEmpty())
            {
                ruleContext = new RuleContext
                {
                    FormCode = formDef.Code,
                    DesignInfo = designInfo,
                    Row = tableData[mainTable][0],
                    TableData = tableData,
                    Table = mainTable,
                    IsAdd = isAdd,
                    FormDataId = id
                };

                ruleInstance = _ruleDefAppService.GetFormRuleInstance(ruleCode);

            }

            if (isNormalSave)
            {
                await InitSysFieldsValue();
            }

            if (ruleInstance != null)
                await ruleInstance.BeforeSave(ruleContext);

            if (ruleContext?.CancelSave != true)
            {
                foreach (var dbTable in designInfo.DbTables)
                {
                    var isMainTable = dbTable.Name == mainTable;
                    var tbName = dbTable.Name;
                    var rows = tableData.GetValueOrDefault(tbName);
                    if (rows == null || rows.Count == 0) continue;

                    InitTableFields(tbName);

                    var isIdentityId = IdFieldIsIdentity(tbName);
                    var refField = GetTableRefField(designInfo, tbName);
                    var isGridTable = tbItemTables.ContainsKey(tbName);
                    var gridRowIds = new List<string>();
                    var isOneRefOneTable = !isMainTable && !isGridTable;

                    if (isGridTable && !isAdd)
                    {
                        gridRowIds = Sql.From(tbName).Where(m => m[refField] == id).Fields(IdField).ToList().Select(m => m[IdField].ToString()).ToList();
                    }

                    foreach (var row in rows)
                    {

                        var idValue = row.GetValueOrDefault(IdField)?.ToString();
                        if (!isAdd && (isMainTable || isOneRefOneTable)) idValue = id;

                        if (idValue.IsNotEmpty())
                        {

                            if (isOneRefOneTable)
                            {
                                var num = Sql.From(tbName).Where(m => m[refField] == id).Update(m => row);
                                if (num == 0)
                                {
                                    if (!isIdentityId)
                                        SetIdValue(row);

                                    row[refField] = id;
                                    SetSysFieldsValue(row, tbName);
                                    Sql.From(tbName).Insert(row);
                                }

                            }
                            else
                            {
                                Sql.From(tbName).Where(m => m[IdField] == idValue).Update(m => row);

                                if (gridRowIds.Count > 0)
                                    gridRowIds.Remove(idValue);
                            }
                        }
                        else
                        {

                            if (isNormalSave)
                            {
                                if (!isIdentityId)
                                {
                                    if (isMainTable)
                                        id = SetIdValue(row, id);
                                    else
                                        SetIdValue(row);
                                }

                                if (!isMainTable)
                                    row[refField] = id;

                                SetSysFieldsValue(row, tbName);
                                Sql.From(tbName).Insert(row);

                                if (isMainTable && isIdentityId)
                                {
                                    id = GetIndentityValue();
                                    if (ruleContext != null)
                                        ruleContext.FormDataId = id;
                                }

                            }
                            else
                            {
                                foreach (var kv in row)
                                {
                                    var num = Sql.From(tbName).Where(m => m[IdField] == kv.Key).Update(m => new { kv.Value });
                                    if (num == 0)
                                    {
                                        Sql.From(tbName).Insert(new { Id = kv.Key, kv.Value });
                                    }
                                }

                            }

                        }


                    }

                    if (gridRowIds.Count > 0)
                    {
                        Sql.From(tbName).Where(m => gridRowIds.Contains((string)m[IdField])).Delete();

                    }

                }

            }

            if (ruleInstance != null)
                await ruleInstance.AfterSave(ruleContext);

            return Success(id, "保存成功");


        }


        public async Task<RetResult> DeleteData(DeleteDataDto dto)
        {

            var designInfo = GetDesignInfoByCode(dto.FormCode, out FormDef formDef);
            var ids = dto.Ids;
            var mainTable = designInfo.DbTables[0].Name;

            var ruleCode = formDef.ServerRule;
            RuleContext ruleContext = null;
            BaseRule ruleInstance = null;

            if (ruleCode.IsNotEmpty())
            {
                ruleContext = new RuleContext
                {
                    FormCode = formDef.Code,
                    DesignInfo = designInfo,
                    Table = mainTable,
                    FormDataId = ids
                };

                ruleInstance = _ruleDefAppService.GetFormRuleInstance(ruleCode);

            }

            if (ruleInstance != null)
                await ruleInstance.BeforeDelete(ruleContext);

            if (ruleContext?.CancelSave != true)
            {
                var idArray = ids.SplitString();

                foreach (var tb in designInfo.DbTables)
                {

                    var tbName = tb.Name;

                    if (tbName == mainTable)
                    {
                        Sql.From(tbName).Where(m => idArray.Contains((string)m[IdField])).Delete();
                    }
                    else
                    {
                        Sql.From(tbName).Where(m => idArray.Contains((string)m[tb.RefField])).Delete();
                    }


                }
            }
            if (ruleInstance != null)
                await ruleInstance.AfterDelete(ruleContext);
            return Success("ok", "删除成功");

        }


        [HttpGet("/api/formdef/ImportTemplate/{formCode}")]
        public IActionResult GetImportTemplate(string formCode)
        {

            var designInfo = GetDesignInfoByCode(formCode, out FormDef formDef);
            var cols = designInfo.ListConfig.List.Cols;
            var exportCols = designInfo.ExportConfig.Cols;

            var colList = from col in exportCols
                          join lstCol in cols on col.Field equals lstCol.Field
                          select lstCol;

            if (!colList.Any())
                return new NotFoundResult();


            var excelColumns = new List<DynamicExcelColumn>();
            var index = 0;
            var row = new Dictionary<string, object>();
            foreach (var col in cols)
            {
                var width = (col.Width == 0 ? 150 : col.Width) / 5;
                excelColumns.Add(new DynamicExcelColumn(col.Field) { Index = index++, Name = col.Title, Width = width });
                row[col.Field] = string.Empty;
            }

            var config = new OpenXmlConfiguration
            {
                AutoFilter = false,
                DynamicColumns = excelColumns.ToArray()

            };
            config.TableStyles = TableStyles.None;
            var rows = new List<Dictionary<string, object>>() { row };
            var memoryStream = new MemoryStream();
            memoryStream.SaveAs(rows, configuration: config);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return new FileStreamResult(memoryStream, "application/octet-stream") { FileDownloadName = formDef.Name + ".xlsx" };



        }

        private static object GetItemDisplayValue(object value)
        {

            if (value is Dictionary<string, object> dic)
            {
                if (dic[TextKey] is string[] texts) return string.Join(", ", texts);
                return dic[ValueKey];
            }

            return value;
        }

        public async Task<RetResult> ExportData(PageDataQueryDto dto)
        {

            var designInfo = GetDesignInfoByCode(dto.FormCode, out FormDef formDef);
            var cols = designInfo.ListConfig.List.Cols;
            var exportCols = designInfo.ExportConfig.Cols;

            var colList = from col in exportCols
                          join lstCol in cols on col.Field equals lstCol.Field
                          select lstCol;

            if (!colList.Any())
                return Error("没有设置导出列");


            var result = await GetPageData(dto);
            if (!result.IsSuccess)
                return result;

            var data = ((PageTResult<object>)result.Data).Rows;

            var rows = new List<IDictionary<string, object>>();

            if (data is DataTable table)
            {
                foreach (DataRow row in table.Rows)
                {
                    var dicRow = new Dictionary<string, object>();
                    foreach (var col in colList)
                    {
                        var value = row[col.Field];
                        value = GetItemDisplayValue(value);
                        dicRow[col.Field] = value;

                    }

                    rows.Add(dicRow);

                }


            }
            else
            {

                var dataList = data as List<dynamic>;
                foreach (var row in dataList)
                {
                    var dbRow = row as IDictionary<string, object>;

                    foreach (var col in colList)
                    {
                        var value = dbRow[col.Field];
                        value = GetItemDisplayValue(value);
                        dbRow[col.Field] = value;

                    }

                    rows.Add(dbRow);

                }

            }

            var excelColumns = new List<DynamicExcelColumn>();
            var index = 0;
            foreach (var col in colList)
            {
                var width = (col.Width == 0 ? 150 : col.Width) / 5;
                excelColumns.Add(new DynamicExcelColumn(col.Field) { Index = index++, Name = col.Title, Width = width });
            }

            var config = new OpenXmlConfiguration
            {
                AutoFilter = false,
                DynamicColumns = [.. excelColumns],
                TableStyles = TableStyles.Default
            };

            var fileName = $"/export/{formDef.Name}_{DateTime.Now.ToFileName()}.xlsx";
            string fullFileName = PathHelper.WebPath(fileName);

            var dir = Path.GetDirectoryName(fullFileName);
            Directory.CreateDirectory(dir);
            MiniExcel.SaveAs(fullFileName, rows, configuration: config);
            return Success(fileName, "导出成功");


        }

        private static bool RowDataValueExists(string table, string filed, string value)
        {

            var data = Sql.From(table).Fields("id").Where(m => m[filed] == value).FirstOrDefault();
            return data != null;

        }

        public async Task<RetResult> ImportData(ImportDataDto dto)
        {

            var fullName = PathHelper.WebPath(dto.FileName);

            if (!File.Exists(fullName))
                throw new BusinessException("文件不存在");

            var designInfo = GetDesignInfoByCode(dto.FormCode, out FormDef formDef);

            var cols = designInfo.ListConfig.List.Cols;
            var exportCols = designInfo.ExportConfig.Cols;

            var importMode = designInfo.ExportConfig.ImportMode;

            bool hasUniqueCols = exportCols.Any(m => m.Unique);

            if (!hasUniqueCols && importMode == 2)
            {

                return Error("无唯一列，无法使用修改模式导入");
            }


            var colList = from col in exportCols
                          join lstCol in cols on col.Field equals lstCol.Field
                          select new { LstCol = lstCol, Col = col };

            var dicItems = new Dictionary<string, BaseItem>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in designInfo.AllItems)
            {
                if (item.Field.IsEmpty()) continue;
                dicItems[item.Field] = item;

            }

            DataTable table;

            using (FileStream stream = File.OpenRead(fullName))
            {
                table = MiniExcel.QueryAsDataTable(stream);

            }

            var mainTable = designInfo.DbTables[0].Name;

            InitTableFields(mainTable);

            await InitSysFieldsValue();

            var index = 1;

            var rowNum = 0;

            foreach (DataRow row in table.Rows)
            {
                var dicRow = new Dictionary<string, object>();
                var isValid = true;
                var isAdd = true;
                var query = Sql.From(mainTable);
                foreach (var col in colList)
                {

                    var field = col.Col.Field;
                    var item = dicItems.GetOrDefault(field);

                    var title = col.LstCol.Title;

                    var isUnique = col.Col.Unique;

                    var value = table.Columns.Contains(title) ? row[title] : null;
                    if (value == DBNull.Value) value = null;

                    if (item != null && item.Required && (value == null || value.ToString().IsEmpty()))
                    {
                        throw new BusinessException($"第{index}行,{title}不能为空");

                    }

                    if (value != null && value.ToString().IsNotEmpty() && item is DataSourceItem dataSourceItem)
                    {
                        var values = await ConvertItemTextOrValue(value.ToString(), dataSourceItem, false);
                        if (values != null)
                        {
                            value = string.Join(",", values);
                        }
                    }

                    dicRow[field] = value;

                    if (isUnique)
                    {
                        var isNull = value == null || value.ToString().IsEmpty();
                        if (isNull)
                        {
                            isValid = false;
                            break;
                        }

                        var strValue = value.ToString();
                        var isExist = RowDataValueExists(mainTable, field, strValue);
                        if (isExist && importMode == 1)
                        {
                            isValid = false;
                            break;
                        }
                        else if (!isExist && importMode == 2)
                        {
                            isValid = false;
                            break;
                        }

                        if (isExist && (importMode == 2 || importMode == 3))
                        {
                            isAdd = false;
                            dicRow.Remove(field);
                            query = query.Where(m => m[field] == strValue);
                        }

                    }


                }
                if (isValid && dicRow.Count > 0)
                {
                    if (isAdd)
                    {
                        dicRow[IdField] = GetIdValue();
                        SetSysFieldsValue(dicRow, mainTable);
                        query.Insert(dicRow);
                    }
                    else
                    {
                        query.Update(m => dicRow);
                    }
                    rowNum++;
                }

                index++;

            }

            return Success(null, $"共导入{rowNum}行");

        }

        public RetResult SetItemValue(SetItemValueDto dto)
        {

            var designInfo = GetDesignInfoByCode(dto.FormCode, out _);
            var item = designInfo.AllItems.FirstOrDefault(m => m.Name == dto.ItemName);
            if (item == null) return Error("控件不存在");
            if (item is Table) return Error("表格控件无法设置");


            var idArray = dto.Ids.SplitString();

            Sql.From(item.Table).Where(m => idArray.Contains((string)m["id"])).Update(m => new Dictionary<string, object> { { item.Field, dto.Value } });

            return Success("操作成功");

        }


        public override async Task<FormDef> Get(int id)
        {
            var result = await base.Get(id);
            result.ServerRule = null;
            return result;
        }


        public async Task<FormDef> GetFullModel(int id)
        {
            var result = await base.Get(id);
            return result;
        }



    }
}
