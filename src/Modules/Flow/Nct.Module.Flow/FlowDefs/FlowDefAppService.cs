using Nct.Module.Flow.Engine;
using Nct.Module.Flow.Engine.Models;
using Nct.Module.Flow.FlowDefs.Dtos;
using Nct.Module.Form.FormDefs;
using Nct.Module.Form.FormDefs.Dtos;
using Nct.Scripting;
using System.Text;
using static Nct.Module.Flow.Consts;

namespace Nct.Module.Flow.FlowDefs
{


    public class FlowDefAppService : CrudAppService<FlowDef, FlowDefDto, FormDefDto, CreateFlowDefDto>, IFlowDefAppService
    {
        private const string CacheKeyPrefix = "flowdef_";
        private readonly Script _script;
        private readonly IFormDefAppService _formDefAppService;
        public FlowDefAppService(IRepository<FlowDef> repository, Script script, IFormDefAppService formDefAppService) : base(repository)
        {
            _script = script;
            _formDefAppService = formDefAppService;
        }

        protected override void SetPermissionNames()
        {
            CreatePermissionName = "addflowdef";
            DeletePermissionName = "delflowdef";
            UpdatePermissionName = "editflowdef";

        }

        private void CheckScript(string script)
        {


            var sb = new StringBuilder();
            sb.AppendLine(_script.BuildImportString());
            sb.AppendLine($"public class Test");
            sb.AppendLine("{");
            sb.AppendLine("public bool GetBool(dynamic context) {");
            sb.AppendLine(script);
            sb.AppendLine("}}");

            var result = _script.Compile(sb.ToString(), null, true);
            if (!result.IsSuccess)
                throw new BusinessException(result.Msg);



        }

        private void CheckNodeScript(string config)
        {

            var configObj = config.ToObject<FlowConfig>();
            var startNodeProps = configObj.Nodes.First(m => m.Type == NodeType.Start).Properties.To<StartNodeProps>();
            var formCode = startNodeProps.FormCode;
            string table = null;

            foreach (var node in configObj.Nodes)
            {
                if (node.Type == NodeType.Script)
                {
                    var props = node.Properties.To<ScriptNodeProps>();

                    try
                    {
                        CheckScript(props.Code);
                    }
                    catch (Exception ex)
                    {
                        throw new BusinessException($"{node.Text.Value}节点，脚本错误：{ex.Message}");
                    }
                }
                else if (node.Type == NodeType.Case)
                {
                    var props = node.Properties.To<CaseNodeProps>();
                    foreach (var cdt in props.Cdts)
                    {

                        if (cdt.SqlCdt.IsNotEmpty())
                        {
                            table ??= _formDefAppService.GetDesignInfoByCode(formCode, out _).DbTables[0].Name;
                            var sql = $"select top 1 id from {table} where (({cdt.SqlCdt}))";
                            try
                            {
                                Db.ExecuteScalar<string>(sql);
                            }
                            catch (Exception ex)
                            {
                                throw new BusinessException($"{node.Text.Value}节点({cdt.Text})，Sql脚本错误：{ex.Message}");
                            }

                        }


                        if (cdt.CodeCdt.IsEmpty()) continue;

                        try
                        {
                            CheckScript(cdt.CodeCdt);
                        }
                        catch (Exception ex)
                        {
                            throw new BusinessException($"{node.Text.Value}节点({cdt.Text})，C#脚本错误：{ex.Message}");
                        }

                    }
                }

            }

        }



        private void CheckCodeExist(CreateFlowDefDto dto)
        {
            var model = Repository.GetAll().Where(m => m.Code == dto.Code && m.Id != dto.Id).FirstOrDefault();
            if (model != null)
                throw new BusinessException("编码已经存在");


        }



        private void RemoveFlowDefCache(string code)
        {

            var key = CacheKeyPrefix + code.ToLower();
            Cache.Remove(key);
        }

        public FlowDef GetByCode(string code)
        {
            Check.NotNullOrEmpty(code, nameof(code));
            var key = CacheKeyPrefix + code.ToLower();
            return Cache.Get(key, () =>
            {
                return Repository.GetAll().Where(m => m.Code == code).FirstOrDefault();

            });

        }

        public override async Task<FlowDefDto> Update(CreateFlowDefDto dto)
        {

            CheckNodeScript(dto.Data);
            var model = await Get(dto.Id);
            if (model == null) return null;

            dto.Code = model.Code;
            var ret = await base.Update(dto);
            RemoveFlowDefCache(dto.Code);
            return ret;
        }


        protected override IQueryable<FlowDef> CreateQuery(FormDefDto dto)
        {
            return base.CreateQuery(dto).WhereIf(m => m.Name.Contains(dto.Name), dto.Name.IsNotEmpty())
            .WhereIf(m => m.Code.Contains(dto.Code), dto.Code.IsNotEmpty())
                .WhereIf(m => m.Descpt.Contains(dto.Descpt), dto.Descpt.IsNotEmpty());
        }

        public override Task<FlowDefDto> Create(CreateFlowDefDto dto)
        {
            CheckCodeExist(dto);
            CheckNodeScript(dto.Data);
            return base.Create(dto);
        }


        public async Task<object> GetAllSimpleList()
        {
            var list = await Repository.GetAllListAsync();
            return list.Select(m => new { m.Id, m.Name, m.Code }).ToList();

        }



    }
}
