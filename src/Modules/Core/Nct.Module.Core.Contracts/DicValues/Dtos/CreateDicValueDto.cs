using System;
using System.ComponentModel.DataAnnotations;
using Nct.ObjectMapper;
using Nct.Application;
using System.Collections.Generic;

namespace Nct.Module.Core.DicValues.Dtos
{

    [Map(typeof(DicValue))]
    public class CreateDicValueDto : IEntityDto
    {

        public int Id { get; set; }

        [Required]
        [StringLength(2000)]
        [Display(Name = "值")]
        public string Value { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "名称")]
        public string Name { get; set; }


        [StringLength(20)]
        [Display(Name = "颜色")]
        public string Color { get; set; }

        [StringLength(200)]
        [Display(Name = "描述")]
        public string Descpt { get; set; }


        [Display(Name = "排序")]
        public int Sort { get; set; }


    }

    public class MultiCreateDicValueDto
    {

        public int DicId { get; set; }

        public string Name { get; set; }
 
        public List<CreateDicValueDto> Values { get; set; }    


    }


}
