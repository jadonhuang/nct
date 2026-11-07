using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Application;
using Nct.Module.Flow.Flows.Dtos;

namespace Nct.Module.Flow.Flows
{    

    public interface IFlowAppService: ICrudAppService<Flow,FlowDto, FlowDto, CreateFlowDto>
    {

        Task HandleTimeoutTask();
    }
}
