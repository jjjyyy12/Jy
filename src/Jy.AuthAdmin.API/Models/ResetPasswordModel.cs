using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.AuthAdmin.API.Models
{
    public class ResetPasswordModel
    {
        public Guid resetPasswordId { get; set; }

        [Required(ErrorMessage = "原密码不能为空。")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "新密码不能为空。")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
 
        [Required(ErrorMessage = "新密码确认不能为空。")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "新密码和确认新密码不匹配。")]
        public string NewPassword2 { get; set; }
    }
}
