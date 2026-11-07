using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data;
using Nct.Domain.Repositories;
using Nct.Application.Form.RuleDefs.Dtos;
using Nct.Extentions;
using Nct.Domain.Entities;
using Nct.ObjectMapper;
using Nct.Json;
using Nct.Application;
using Nct.LinqQuery;
using Nct.Module.Form.DataSources.Dtos;
using Nct.Module.Core.DicValues;
using Nct.Scripting;
using System.Text;
using System.Reflection;
using Nct.Module.Form.RuleDefs;
using Nct.Authorization;

namespace Nct.Application.Form.RuleDefs
{


    public class RuleDefAppService : CrudAppService<RuleDef, RuleDefDto, RuleDefDto, CreateRuleDefDto>, IRuleDefAppService
    {

        const string FormRuleClassName = "FormRuleClass";
        private readonly IDicValueAppService _dicValueAppService;
        private readonly Script _script;
        public RuleDefAppService(IRepository<RuleDef> repository, IDicValueAppService dicValueAppService, Script script) : base(repository)
        {

            _dicValueAppService = dicValueAppService;
            _script = script;
        }

        protected override void SetPermissionNames()
        {
            CreatePermissionName = "addruledef";
            DeletePermissionName = "delruledef";
            UpdatePermissionName = "editruledef";

        }


        protected override IQueryable<RuleDef> CreateQuery(RuleDefDto dto)
        {
            return base.CreateQuery(dto).WhereIf(m => m.Name.Contains(dto.Name), dto.Name.IsNotEmpty())
                .WhereIf(m => m.Cate == dto.Cate, dto.Cate.IsNotEmpty())
                .WhereIf(m=>m.Type==dto.Type,dto.Type>0);
        }

        public override async Task<PageResult<RuleDefDto>> GetList(PageQuery<RuleDefDto> pageQuery)
        {
            pageQuery.OrderBy = "Id asc";
            var list = await base.GetList(pageQuery);
            var dicValues = await _dicValueAppService.GetValuesByCode("rulecate");

            foreach (var item in list.Rows)
            {
                var dicValue = dicValues.FirstOrDefault(m => m.Value == item.Cate);
                if (dicValue != null)
                    item.CateName = dicValue.Name;


            }
            return list;
        }

        public string BuildFormRuleClassCode(string code)
        {
            var sb = new StringBuilder();
            sb.AppendLine(_script.BuildImportString());
            sb.AppendLine($"public class {FormRuleClassName}:BaseRule");
            sb.AppendLine("{");
            sb.AppendLine(code);
            sb.AppendLine("}");
            return sb.ToString();

        }

        public void TryCompileFormRuleClassCode(string code)
        {
            code = BuildFormRuleClassCode(code);
            var result = _script.Compile(code, null, true);
            if (!result.IsSuccess)
                throw new BusinessException(result.Msg);

        }

        public BaseRule GetFormRuleInstance(string code)
        {
            code = BuildFormRuleClassCode(code);
            Assembly assembly = _script.GenerateOrGetAssemblyFromCode(code, true);
            var instance = assembly.CreateInstance(FormRuleClassName);
            return (BaseRule)instance;

        }



    }
}
