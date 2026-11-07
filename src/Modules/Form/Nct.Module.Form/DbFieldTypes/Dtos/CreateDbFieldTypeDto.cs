using System;
using System.ComponentModel.DataAnnotations;
using Nct.ObjectMapper;
using Nct.Application;
using Nct.Validation;

namespace Nct.Module.Form.DbFieldTypes.Dtos
{    
  
    [Map(typeof(DbFieldType))]
    public class CreateDbFieldTypeDto:IEntityDto
    {

        public int Id { get; set; }


        [Required]
        [StringLength(50)]
        [Display(Name = "字段类型")]
        [CustomValidate(ValidateType.ValidString)]
        public string Name { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "类型说明")]
        public string Descpt { get; set; }


        [Required]
		[StringLength(50)]  
		[Display(Name ="数据库字段类型")]
		public string DataType { get; set; }



        [Display(Name = "长度")]
        public int Length { get; set; }

        [Display(Name = "精度")]
        public int Precision { get; set; }




    }
}
