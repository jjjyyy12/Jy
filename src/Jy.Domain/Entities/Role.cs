﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Jy.IRepositories;

namespace Jy.Domain.Entities
{
    public class Role : Entity
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public Guid CreateUserId { get; set; }

        public DateTime? CreateTime { get; set; }
        public virtual User CreateUser { get; set; }
        public string Remarks { get; set; }

        //public virtual ICollection<User> Users { get; set; }

        //public virtual ICollection<Menu> Menus { get; set; }

        public virtual ICollection<RoleMenu> RoleMenus { get; set; }

        public virtual ICollection<UserRole> RoleUsers { get; set; }
    }
}
