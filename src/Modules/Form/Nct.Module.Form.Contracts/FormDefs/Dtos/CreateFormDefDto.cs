using System;
using System.ComponentModel.DataAnnotations;
using Nct.ObjectMapper;
using Nct.Application;
using Nct.Validation;

namespace Nct.Module.Form.FormDefs.Dtos
{    
  
    [Map(typeof(FormDef))]
    public class CreateFormDefDto:IEntityDto
    {


		public int AutoCreateTable { get; set; }

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

		[StringLength(100)]  
		[Display(Name ="表单描述")]
		public string Descpt { get; set; }

  
		[Display(Name ="设计数据")]
		public string Data { get; set; }

        public string ClientRule { get; set; }

        public string ServerRule { get; set; }

    }
}
