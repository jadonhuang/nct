using System;
using System.ComponentModel.DataAnnotations;
using Nct.ObjectMapper;
using Nct.Application;
using Nct.Validation;

namespace Nct.Module.Core.Dictionarys.Dtos
{    
  
    [Map(typeof(Dictionary))]
    public class CreateDictionaryDto:IEntityDto
    {
  
		
		public int Id { get; set; }
		
		public int ParentId { get; set; }

 
        [StringLength(50)]  
		[Display(Name ="编码")]
        [CustomValidate(ValidateType.LetterAndNumber)]
        public string Code { get; set; }

		[Required]
		[StringLength(50)]  
		[Display(Name ="名称")]
        [CustomValidate(ValidateType.ValidString)]
        public string Name { get; set; }


		[StringLength(255)]  
		[Display(Name ="描述")]
		public string Descpt { get; set; }

  
		[Display(Name ="排序")]
		public int Sort { get; set; }

  

    }
}
