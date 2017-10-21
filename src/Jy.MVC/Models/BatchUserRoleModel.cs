using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.MVC.Models
{
    public class BatchUserRoleModel
    {
        [Required(ErrorMessage = "userIDs不能为空。")]
        public string userIDs { get; set; }
        public string roleIDs { get; set; }
    }
}
