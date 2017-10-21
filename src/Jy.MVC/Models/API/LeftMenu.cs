using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.MVC.Models
{
    public class LeftMenu
    {
        public List<RoleMenu> Menus { set; get; }
        public string UserName { set; get; }
        public string Name { set; get; }
        public string DepartmentName { set; get; }
        public string LoginTime { set; get; }
        public string Email { set; get; }
        public string Mobile { set; get; }
    }
}
