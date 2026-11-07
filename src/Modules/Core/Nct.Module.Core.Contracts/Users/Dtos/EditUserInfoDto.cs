using Nct.ObjectMapper;
using Nct.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Core.Users.Dtos
{

    [Map(typeof(User))]
    [Map(typeof(UserDto))]
    public class EditUserInfoDto 
    {
      
       
        public string UserName { get; set; }


        [Required]
        [StringLength(20)]
        [Display(Name = "姓名")]
        [CustomValidate(ValidateType.ValidString)]
        public string RealName { get; set; }


        [StringLength(50)]
        [Display(Name = "手机")]
        [CustomValidate( ValidateType.Mobile)]
        public string Tel { get; set; }


        [Display(Name = "性别")]
        public int Sex { get; set; }


        [StringLength(50)]
        [Display(Name = "联系地址")]
        public string Address { get; set; }


        public string Pic { get; set; }


    }
}
