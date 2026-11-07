using Nct.Authorization;
using Nct.DataAccess.Json;
using Nct.Models;
using Nct.Module.Form.DataSources.Dtos;
using Nct.Scripting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nct.Module.Form.DataSources
{


    public class DataSourceAppService : CrudAppService<DataSource, DataSourceDto, DataSourceDto, CreateDataSourceDto>, IDataSourceAppService
    {
        private const string CacheKeyPrefix = "ds_";
        private readonly Script _script;
        private readonly ISqlJsonParser _sqlJsonParser;

        public DataSourceAppService(IRepository<DataSource> repository, Script script
            , ISqlJsonParser sqlJsonParser
            ) : base(repository)
        {
            _script = script;
            _sqlJsonParser = sqlJsonParser;
        }


        protected override void SetPermissionNames()
        {
            CreatePermissionName = "adddatasource";
            DeletePermissionName = "deldatasource";
            UpdatePermissionName = "editdatasource";

        }


        private void CheckCodeExist(CreateDataSourceDto dto)
        {
            var model = Repository.GetAll().Where(m => m.Code == dto.Code && m.Id != dto.Id).FirstOrDefault();
            if (model != null)
                throw new BusinessException("编码已经存在");


        }

        private void CheckScript(CreateDataSourceDto dto)
        {
            if (dto.Type == 2)
            {

                var result = _script.Compile(dto.Data);
                if (!result.IsSuccess)
                    throw new BusinessException(result.Msg);

            }

        }


        public override async Task<DataSourceDto> Create(CreateDataSourceDto dto)
        {
            CheckCodeExist(dto);
            CheckScript(dto);
            var result = await base.Create(dto);
            RemoveCache(result.Code);
            return result;
        }



        public override async Task<DataSourceDto> Update(CreateDataSourceDto dto)
        {
            CheckCodeExist(dto);
            CheckScript(dto);
            var result = await base.Update(dto);
            RemoveCache(result.Code);
            return result;
        }



        public override async Task<int> Delete(int id)
        {
            var entity = await Get(id);
            RemoveCache(entity?.Code);
            return await base.Delete(id);
        }

        protected override IQueryable<DataSource> CreateQuery(DataSourceDto dto)
        {

            return base.CreateQuery(dto)
              .WhereIf(m => m.Code.Contains(dto.Code), dto.Code.IsNotEmpty())
              .WhereIf(m => m.Cate == dto.Cate, dto.Cate > 0)
               .WhereIf(m => m.Name.Contains(dto.Name), dto.Name.IsNotEmpty());
        }

        private static SqlExtraInfo GetSqlExtraInfo(string sql)
        {

            var extraInfo = new SqlExtraInfo();
            var regex = new Regex(@"/\*(.+)\*/");

            foreach (Match m in regex.Matches(sql))
            {
                string value = m.Groups[1].Value;
                if (value.IgnoreCaseContains("order ") && value.IgnoreCaseContains(" by"))
                {
                    value = Regex.Replace(value, "\\s*order\\s+by", "", RegexOptions.IgnoreCase);
                    extraInfo.OrderBy = value;
                }
                else if (value.IgnoreCaseContains("#nowhere#"))
                {
                    extraInfo.IsNoWhere = true;

                }

            }

            sql = regex.Replace(sql, "");
            sql = sql.TrimStart(' ', '\r', '\n', '\t');
            extraInfo.IsSimple = sql.StartsWith("select", StringComparison.OrdinalIgnoreCase);
            return extraInfo;

        }


        private object GetScriptResult(string scriptStr, DsDataQueryDto queryDto, dynamic args)
        {

            var isPage = queryDto is DsDataPageQueryDto;
            args._query = queryDto;
            args._isPage = isPage;
            args._fields = CallContext.GetData("_fields") as string;

            object result = _script.Evaluate<object>(scriptStr, args);

            if (!isPage && result is DataTable table)
            {
                return table;
            }

            if (isPage && result is PageTableResult pageTable)
            {
                return pageTable;
            }

            throw new Exception("return type is error");

        }

        private static void SetSqlParamsDefaultValue(string sql, IDictionary<string, object> @params)
        {

            var matches = Regex.Matches(sql, "@\\w+");

            foreach (Match m in matches)
            {
                var name = m.Value;
                var name2 = name.TrimStart('@');
                if (!@params.ContainsKey(name) && !@params.ContainsKey(name2))
                    @params[name2] = null;

            }

        }

        private IDictionary<string, object> BuildParams(ref string sql, DsDataQueryDto queryDto, out SqlExtraInfo sqlInfo, bool getCols = false)
        {

            sqlInfo = null;
            var query = queryDto.Query;
            IDictionary<string, object> args = new ExpandoObject();
            ConditionInfo conditionInfo = null;
            JsonTable jsonTable = null;
            string orderBy = null;
            var summaryFields = CallContext.GetData(Consts.SummaryFields) as string;
            if (query.IsNotEmpty())
            {
                query = "{\"Tb\":" + query + "}";
                jsonTable = _sqlJsonParser.Parse(query).First();
                conditionInfo = new DataSourceJsonSqlCreater().GetConditionInfo(jsonTable);
                foreach (var kv in conditionInfo.Parameters)
                {
                    args[kv.Key] = kv.Value;
                }
                if (jsonTable.OrderBys.Count > 0)
                {
                    orderBy = string.Join(",", jsonTable.OrderBys.Select(m => $"[{m.Key}] {m.Value}"));
                }
                args["_where"] = conditionInfo.Condition;
            }
            args["_getCols"] = getCols;
            args["_uid"] = Session.UserId;
            args[Consts.SummaryFields] = summaryFields;

            if (sql != null)
            {
                sqlInfo = GetSqlExtraInfo(sql);
                if (sqlInfo.OrderBy.IsEmpty())
                {
                    sqlInfo.OrderBy = orderBy;
                }
                else
                {
                    orderBy = sqlInfo.OrderBy;
                }

                if (sqlInfo.IsSimple)
                {

                    if (!sqlInfo.IsNoWhere)
                    {
                        string where = null;

                        if (conditionInfo != null && conditionInfo.Condition.IsNotEmpty())
                        {
                            where = conditionInfo.Condition;
                        }

                        SqlHelper.AppendContextExtraWhere(ref where);

                        if (where.IsNotEmpty())
                        {
                            sql = $"select * from ({sql}) t {where}";
                        }

                    }


                    if (summaryFields.IsNotEmpty())
                    {
                        sql = $"select {summaryFields} from ({sql}) tb";
                    }

                    SetSqlParamsDefaultValue(sql, args);
                }


            }

            args["_orderBy"] = orderBy;

            if (queryDto is DsDataPageQueryDto dto)
                dto.OrderBy = orderBy;
            return args;


        }

        private void RemoveCache(string code)
        {
            if (code == null) return;
            Cache.Remove(CacheKeyPrefix + code.ToLower());

        }


        private DataSource GetByCode(string code)
        {

            var key = CacheKeyPrefix + code.ToLower();

            return Cache.Get(key, () =>
            {

                var model = Repository.GetAll().Where(x => x.Code == code).FirstOrDefault();
                return model;

            });

        }



        public DataTable GetDataTable(DsDataQueryDto dto)
        {

            var model = GetByCode(dto.Code) ?? throw new ArgumentNullException("dsmodel");

            string sql = model.IsScript ? null : model.Data;

            var args = BuildParams(ref sql, dto, out SqlExtraInfo sqlExtraInfo);

            if (model.IsScript)
            {
                return GetScriptResult(model.Data, dto, args) as DataTable;
            }

            if (sqlExtraInfo.OrderBy.IsNotEmpty())
            {
                sql = $"{sql} order by {sqlExtraInfo.OrderBy}";

            }

            return Db.QueryTable(sql, args);

        }


        public PageTableResult GetDataTablePage(DsDataPageQueryDto dto)
        {

            var model = GetByCode(dto.Code) ?? throw new ArgumentNullException("dsmodel");

            string sql = model.IsScript ? null : model.Data;

            var args = BuildParams(ref sql, dto, out _);

            PageTableResult pageData;

            if (model.IsScript)
            {
                pageData = GetScriptResult(model.Data, dto, args) as PageTableResult;
            }
            else
            {
                pageData = Db.QueryTable(sql, dto, args);
            }
            pageData.PageIndex = dto.PageIndex;
            pageData.PageSize = dto.PageSize;

            return pageData;

        }


        public List<NameValue> GetColumns(string code)
        {

            var model = GetByCode(code) ?? throw new ArgumentNullException("dsmodel");

            var cols = new List<NameValue>();

            string sql = model.IsScript ? null : model.Data;

            var args = BuildParams(ref sql, new DsDataQueryDto(), out SqlExtraInfo sqlInfo, true);

            if (model.IsScript)
            {

                object result = _script.Evaluate<object>(model.Data, args);
                if (result is IDictionary<string, string> dic)
                {
                    foreach (var kv in dic)
                    {
                        cols.Add(new NameValue { Name = kv.Value, Value = kv.Key });

                    }
                }

            }
            else
            {

                if (sqlInfo.IsSimple && !sql.Contains("@_getCols"))
                {
                    sql = string.Format(" select t.* From ({0})t where 1>1", sql);

                }

                DataTable dt = Db.QueryTable(sql, args);

                bool hasDes = dt.Rows.Count == 1;

                foreach (DataColumn col in dt.Columns)
                {

                    if (col.ColumnName.IgnoreCaseEqual(SqlHelper.RowIndexName)) continue;

                    var name = col.ColumnName;

                    var nameValue = new NameValue { Name = name, Value = name };

                    string colText;

                    if (hasDes && col.DataType == typeof(string) && (colText = dt.Rows[0][name].ToString()).IsNotEmpty())
                    {
                        nameValue.Name = colText;
                    }
                    cols.Add(nameValue);

                }

            }

            return cols;


        }




    }
}
