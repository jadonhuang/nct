using System;
using System.ComponentModel.DataAnnotations;
using Nct.ObjectMapper;
using Nct.Application;
using Nct.Validation;

namespace Nct.Module.Core.Menus.Dtos
{    
  
    [Map(typeof(Menu))]
    public class CreateMenuDto:IEntityDto
    {
  
		
		public int Id { get; set; }


		public int ParentId { get; set; }

		[Required]
		[StringLength(50)]  
		[Display(Name ="编码")]
        [CustomValidate(ValidateType.LetterAndNumber)]
        public string Code { get; set; }

		[Required]
		[StringLength(100)]  
		[Display(Name ="名称")]
        [CustomValidate(ValidateType.ValidString)]
        public string Name { get; set; }


		[StringLength(500)]  
		[Display(Name ="链接")]
		public string Url { get; set; }

  
		[Display(Name ="排序")]
		public int Sort { get; set; }


		[StringLength(50)]  
		[Display(Name ="图标")]
		public string Icon { get; set; }

 
  
		[Display(Name ="是否显示")]
		public bool IsShow { get; set; }

  
		[Display(Name ="是否菜单")]
		public bool IsMenu { get; set; }


    }
}
