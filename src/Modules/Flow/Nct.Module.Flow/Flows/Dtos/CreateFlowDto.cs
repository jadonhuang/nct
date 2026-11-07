using System;
using System.ComponentModel.DataAnnotations;
using Nct.ObjectMapper;
using Nct.Application;

namespace Nct.Module.Flow.Flows.Dtos
{    
  
    public class CreateFlowDto:IEntityDto
    {
  
		public int Id { get; set; }

  
		public string FlowCode{ get; set; }

        public string FormData { get; set; }

        public string FormDataId { get; set; }

    }
}
