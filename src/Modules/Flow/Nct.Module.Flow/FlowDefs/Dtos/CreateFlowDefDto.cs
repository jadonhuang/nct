using System;
using System.ComponentModel.DataAnnotations;
using Nct.ObjectMapper;
using Nct.Application;
using Nct.Validation;

namespace Nct.Module.Flow.FlowDefs.Dtos
{    
  
    [Map(typeof(FlowDef))]
    public class CreateFlowDefDto:IEntityDto
    {
  

		public int Id { get; set; }

		[Required]
		[StringLength(50)]  
		[Display(Name ="名称")]
		[CustomValidate(ValidateType.ValidString)]
		public string Name { get; set; }


		[StringLength(50)]
        [CustomValidate(ValidateType.LetterAndNumber)]
        [Display(Name ="编号")]
		public string Code { get; set; }


		[StringLength(100)]  
		[Display(Name ="描述")]
		public string Descpt { get; set; }



		[StringLength(200)]  
		[Display(Name ="流程管理员")]
		public string AdminIds { get; set; }

  
		[Display(Name ="申请人配置")]
		public string Applicant { get; set; }

  
		[Display(Name ="设计数据")]
		public string Data { get; set; }




    }
}
