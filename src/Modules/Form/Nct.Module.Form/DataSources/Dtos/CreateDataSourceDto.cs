using System;
using System.ComponentModel.DataAnnotations;
using Nct.ObjectMapper;
using Nct.Application;
using Nct.Validation;

namespace Nct.Module.Form.DataSources.Dtos
{    
  
    [Map(typeof(DataSource))]
    public class CreateDataSourceDto:IEntityDto
    {
  
	
		public int Id { get; set; }

		[Required]
		[StringLength(100)]  
		[Display(Name ="名称")]
        [CustomValidate(ValidateType.ValidString)]
        public string Name { get; set; }

		[Required]
		[StringLength(50)]  
		[Display(Name ="编码")]
        [CustomValidate(ValidateType.LetterAndNumber)]
        public string Code { get; set; }

  
		[Display(Name ="分类")]
		public int Cate { get; set; }

  

		[StringLength(100)]  
		[Display(Name ="表单描述")]
		public string Descpt { get; set; }

  
		[Display(Name ="数据源类型")]
		[Range(1,2)]
		public int Type { get; set; }

		[Required]  
		[Display(Name ="数据")]
		public string Data { get; set; }


    }
}
