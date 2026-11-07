using Nct.Application;
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
    public class CreateUserDto:IEntityDto
    {
        public int Id { get; set; }


        [Required]
        [StringLength(50)]
        [Display(Name = "用户名")]
        [CustomValidate(ValidateType.LetterAndNumber)]
        public string UserName { get; set; }

        [RequiredIf("Id",0)]
        [StringLength(20,ErrorMessage ="密码长度6-20",MinimumLength =6)]

        [Display(Name = "密码")]
        public string Password { get; set; }

        public bool IsActive { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "姓名")]
        [CustomValidate(ValidateType.ValidString)]
        public string RealName { get; set; }


        [CustomValidate(ValidateType.Mobile,ErrorMessage ="请输入正确的手机号")]
        [Display(Name = "手机")]
        public string Tel { get; set; }


        [Display(Name = "性别")]
        public int Sex { get; set; }


        [StringLength(50)]
        [Display(Name = "联系地址")]
        public string Address { get; set; }

        public List<int> RoleIds    { get; set; }

        public int? OrgId { get; set; }




    }
}
