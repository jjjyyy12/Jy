using Jy.Domain.Entities;
using Jy.Domain.IRepositories;
using Jy.EntityFramewordCoreBase.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
 
namespace Jy.EntityFrameworkCore.Repositories
{
    public class RoleRepositoryRead : JyRepositoryReadBase<Role, JyDbContext>, IRoleRepositoryRead
    {
        public RoleRepositoryRead(JyDbContext dbcontext) : base(dbcontext)
        {

        }
        /// <summary>
        /// 获取角色下的菜单
        /// </summary>
        /// <param name="id">roleid</param>
        public List<RoleMenu> GetRoleMenus(Guid id)//加载子对象问题
        {
            //return _dbContext.RoleMenus.Where(it => it.RoleId == id).ToList();
            var query = from x in _dbContext.RoleMenus
                        where id.Equals(x.RoleId)
                     join y in _dbContext.Menus on x.MenuId equals y.Id into temp
                     from y in temp.DefaultIfEmpty()
                     join z in _dbContext.Roles on x.RoleId equals z.Id into temp1
                     from z in temp1.DefaultIfEmpty()
                        select new RoleMenu() { MenuId=x.MenuId,RoleId=x.RoleId,Menu=y,Role=z };
            var dd = query.Cast<RoleMenu>().ToList();
            return dd;
        }
        /// <summary>
        /// 获取当前用户的菜单
        /// </summary>
        /// <param name="id">userid</param>
        /// <returns></returns>
        public List<RoleMenu> GetUserRoleMenus(Guid id)
        {
            var query = from u in _dbContext.UserRoles
                            where id.Equals(u.UserId)
                        join x in _dbContext.RoleMenus on u.RoleId equals x.RoleId   
                        join y in _dbContext.Menus on x.MenuId equals y.Id into temp
                        from y in temp.DefaultIfEmpty()
                        join z in _dbContext.Roles on x.RoleId equals z.Id into temp1
                        from z in temp1.DefaultIfEmpty()
                        select new RoleMenu() { MenuId = x.MenuId, RoleId = x.RoleId, Menu = y, Role = z };
            var dd = query.Cast<RoleMenu>().ToList();
            return dd;
        }
        public List<RoleMenu> GetAllRoleMenus()
        {
            var query = from x in _dbContext.RoleMenus
                        join y in _dbContext.Menus on x.MenuId equals y.Id into temp
                        from y in temp.DefaultIfEmpty()
                        join z in _dbContext.Roles on x.RoleId equals z.Id into temp1
                        from z in temp1.DefaultIfEmpty()
                        select new RoleMenu() { MenuId = x.MenuId, RoleId = x.RoleId, Menu = y, Role = z };
            var dd = query.Cast<RoleMenu>().ToList();
            return dd;
        }
     
    }
}
