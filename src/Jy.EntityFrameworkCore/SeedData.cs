using Jy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.EntityFrameworkCore
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new JyDbContext(serviceProvider.GetRequiredService<DbContextOptions<JyDbContext>>()))
            {
                if (context.Users.Any())
                {
                    return;   // 已经初始化过数据，直接返回
                }
                Guid departmentId = Guid.NewGuid();
                //增加一个部门
                context.Departments.Add(
                   new Department
                   {
                       Id = departmentId,
                       Name = "总部",
                       ParentId = Guid.Empty
                   }
                );
                //角色
                Guid roleId = Guid.NewGuid();
                context.Roles.Add(
                   new Role
                   {
                       Id = roleId,
                       Name = "admin",
                       Code = "admin"
                   }
                );
                //增加一个超级管理员用户
                Guid userId = Guid.NewGuid();
                context.Users.Add(
                     new User
                     {
                         Id= userId,
                         UserName = "admin",
                         Password = "123456", //暂不进行加密
                         Name = "超级管理员",
                         DepartmentId = departmentId
                     }
                );
                //增加四个基本功能菜单
                Guid menuId1 = Guid.NewGuid();
                Guid menuId2 = Guid.NewGuid();
                Guid menuId3 = Guid.NewGuid();
                Guid menuId4 = Guid.NewGuid();
                Guid menuId5 = Guid.NewGuid();
                context.Menus.AddRange(
                   new Menu
                   {
                       Id = menuId1,
                       Name = "组织机构管理",
                       Code = "Department",
                       SerialNumber = 0,
                       ParentId = Guid.Empty,
                       Icon = "fa fa-link",
                       Url = "/Department/Index"
                   },
                   new Menu
                   {
                       Id = menuId2,
                       Name = "角色管理",
                       Code = "Role",
                       SerialNumber = 1,
                       ParentId = Guid.Empty,
                       Icon = "fa fa-link",
                       Url = "/Role/Index"
                   },
                   new Menu
                   {
                       Id = menuId3,
                       Name = "用户管理",
                       Code = "User",
                       SerialNumber = 2,
                       ParentId = Guid.Empty,
                       Icon = "fa fa-link",
                       Url = "/User/Index"
                   },
                   new Menu
                   {
                       Id = menuId4,
                       Name = "功能管理",
                       Code = "Menu",
                       SerialNumber = 3,
                       ParentId = Guid.Empty,
                       Icon = "fa fa-link",
                       Url = "/Menu/Index"
                   },
                   new Menu
                   {
                       Id = menuId5,
                       Name = "权限管理",
                       Code = "Authority",
                       SerialNumber = 4,
                       ParentId = Guid.Empty,
                       Icon = "fa fa-link",
                       Url = "/Authority/Index"
                   }
                );
                context.RoleMenus.AddRange(new RoleMenu {
                    RoleId=roleId,
                    MenuId = menuId1
                },
                new RoleMenu
                {
                    RoleId = roleId,
                    MenuId = menuId2
                },
                new RoleMenu
                {
                    RoleId = roleId,
                    MenuId = menuId3
                },
                new RoleMenu
                {
                    RoleId = roleId,
                    MenuId = menuId4
                },
                new RoleMenu
                {
                    RoleId = roleId,
                    MenuId = menuId5
                }
                );
                context.UserRoles.AddRange(new UserRole
                {
                    RoleId = roleId,
                    UserId = userId
                }
                );
                context.UserIndexs.AddRange(new UserIndex
                {
                    DepartmentId = departmentId,
                    UserId = userId,
                    UserName = "admin",
                    Password = "123456"
                }
                );
                context.SaveChanges();
            }
        }
    }
}
