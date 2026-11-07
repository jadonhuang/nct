using System;
using System.ComponentModel.DataAnnotations;
using Nct.ObjectMapper;

namespace Nct.Module.Form.Annexs.Dtos
{    
  

   


    [Map(typeof(Annex))]
    public class CreateAnnexDto:IEntityDto<string>
    {
	

		public string Id { get; set; }

        


    }
}
