using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.MVC.Models
{
    public class UserRoleModel
    {
        [Required(ErrorMessage = "userRoleId不能为空。")]
        public Guid userRoleId { get; set; }
        public string roleIDs { get; set; }
    }
}
