using System;
using System.ComponentModel.DataAnnotations;
using Nct.ObjectMapper;
using Nct.Application;

namespace Nct.Application.Form.RuleDefs.Dtos
{    
  
    [Map(typeof(RuleDef))]
    public class CreateRuleDefDto:IEntityDto
    {
  
	
		public int Id { get; set; }

		[Required]
		[StringLength(50)]  
		[Display(Name ="名称")]
		public string Name { get; set; }


		[Required]
		[StringLength(50)]  
		[Display(Name ="类别")]
		public string Cate { get; set; }


		[Range(1,2)]
		[Display(Name ="类型")]
		public int Type { get; set; }



		[StringLength(100)]  
		[Display(Name ="表单描述")]
		public string Descpt { get; set; }

		[Required]  
		[Display(Name ="代码数据")]
		public string Data { get; set; }


    }
}
