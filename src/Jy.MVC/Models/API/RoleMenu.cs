using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.MVC.Models
{
    public class RoleMenu
    {
        public Guid RoleId { get; set; }
        public Guid MenuId { get; set; }

        public string Url { get; set; }
        public string MenuName { get; set; }
    }

}
