using System;
using System.ComponentModel.DataAnnotations;
using Nct.ObjectMapper;
using Nct.Application;
using System.Collections.Generic;
using Nct.Validation;


namespace Nct.Module.Form.DbTables.Dtos
{    
  
    [Map(typeof(DbTable))]
    public class CreateDbTableDto:IEntityDto
    {
  
		public int Id { get; set; }

		[Required]
		[StringLength(100)]  
		[Display(Name ="表名称")]
        [CustomValidate(ValidateType.ValidString)]
        public string Name { get; set; }


        [StringLength(100)]
        [Display(Name = "表单描述")]
        public string Descpt { get; set; }

  
		[Display(Name ="分类")]
		public int Cate { get; set; }

  
		public List<DbFieldDto> Fields { get; set; }


    }
}
