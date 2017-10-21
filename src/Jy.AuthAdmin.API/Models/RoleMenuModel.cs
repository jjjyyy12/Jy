using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.AuthAdmin.API.Models
{
    public class RoleMenuModel
    {
        [Required(ErrorMessage = "roleMenuId不能为空。")]
        public Guid roleMenuId { get; set; }
        public string menuIDs { get; set; }
    }
}
