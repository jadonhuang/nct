using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Application;
using Nct.Application.Form.RuleDefs.Dtos;
using Nct.Module.Form.DataSources.Dtos;
using Nct.Module.Form.RuleDefs;

namespace Nct.Application.Form.RuleDefs
{    

    public interface IRuleDefAppService: ICrudAppService<RuleDef,RuleDefDto,RuleDefDto, CreateRuleDefDto>
    {

        string BuildFormRuleClassCode(string code);

        void TryCompileFormRuleClassCode(string code);

        BaseRule GetFormRuleInstance(string code);

    }
}
