using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Module.Core.Users.Dtos
{
   public class UpdatePassowordDto
    {

        [Required]
        [StringLength(50)]
        [Display(Name = "旧密码")]
        public string OldPassword { get; set; }


        [Required]
        [StringLength(50),MinLength(6,ErrorMessage ="至少6位")]
        [Display(Name = "新密码")]
        public string NewPassword { get; set; }


        [Required]
        [StringLength(50)]
        [Display(Name = "确认密码")]
        [Compare("NewPassword",ErrorMessage ="两次密码输入不一致")]
        public string ConfirmPassword { get; set; }
    }
}
