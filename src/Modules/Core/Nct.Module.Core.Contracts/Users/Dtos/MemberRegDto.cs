
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
   public class MemberRegDto
    {
        [Required]
        [Display(Name = "手机号")]
        [CustomValidate( ValidateType.Mobile)]
        public string UserName { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "长度6-20个字符")]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "两次密码输入不一致")]
        [Display(Name = "确认密码")]
        public string ConfirmPassword { get; set; }


        [Required]
        [Display(Name = "验证码")]
        public string VerifyCode { get; set; }

        public bool Agree { get; set; }

    }
}
