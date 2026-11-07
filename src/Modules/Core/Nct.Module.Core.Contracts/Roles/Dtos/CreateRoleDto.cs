using System;
using System.ComponentModel.DataAnnotations;
using Nct.Application;
using Nct.ObjectMapper;
using Nct.Validation;

namespace Nct.Module.Core.Roles.Dtos
{    
  
    [Map(typeof(Role))]
    public class CreateRoleDto:IEntityDto
    {
  
		
		public int Id { get; set; }

		[Required]
		[StringLength(50)]  
		[Display(Name ="名称")]
        [CustomValidate(ValidateType.ValidString)]
        public string Name { get; set; }

		[Required]
		[StringLength(50)]  
		[Display(Name ="编码")]
        [CustomValidate(ValidateType.LetterAndNumber)]
        public string Code { get; set; }


		
		[StringLength(50)]  
		[Display(Name ="描述")]
		public string Descpt { get; set; }

		public string Permissions { get; set; }




    }
}
