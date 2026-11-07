using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Application;
using Nct.Module.Flow.FlowDefs.Dtos;
using Nct.Module.Form.FormDefs.Dtos;

namespace Nct.Module.Flow.FlowDefs
{    

    public interface IFlowDefAppService: ICrudAppService<FlowDef,FlowDefDto, FormDefDto, CreateFlowDefDto>
    {

        FlowDef GetByCode(string code);
    }
}
