using Jy.DapperBase;
using Jy.DapperBase.Repositories;
using Jy.DapperBase.Repositories.Extensions;
using Jy.Domain.Entities;
using Jy.Domain.IRepositories; 
using Jy.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq; 
 
namespace Jy.Dapper.Repositories
{
    public class RoleRepositoryRead : DapperRepositoryReadBase<Role>, IRoleRepositoryRead
    {
        protected readonly TransactedConnection dbContext;
        public RoleRepositoryRead(IRepositoryReadContext dbcontext) : base(dbcontext)
        {
            dbContext = _dbContext;
        }
        /// <summary>
        /// 获取角色下的菜单
        /// </summary>
        /// <param name="id">roleid</param>
        public List<RoleMenu> GetRoleMenus(Guid id)//加载子对象问题
        {
            //return _dbContext.RoleMenus.Where(it => it.RoleId == id).ToList();
            var query = from x in _dbContext.connection.Query<RoleMenu>()
                        where id.Equals(x.RoleId)
                     join y in _dbContext.connection.Query<Menu>() on x.MenuId equals y.Id into temp
                     from y in temp.DefaultIfEmpty()
                     join z in _dbContext.connection.Query<Role>() on x.RoleId equals z.Id into temp1
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
            var query = from u in _dbContext.connection.Query<UserRole>()
                            where id.Equals(u.UserId)
                        join x in _dbContext.connection.Query<RoleMenu>() on u.RoleId equals x.RoleId   
                        join y in _dbContext.connection.Query<Menu>() on x.MenuId equals y.Id into temp
                        from y in temp.DefaultIfEmpty()
                        join z in _dbContext.connection.Query<Role>() on x.RoleId equals z.Id into temp1
                        from z in temp1.DefaultIfEmpty()
                        select new RoleMenu() { MenuId = x.MenuId, RoleId = x.RoleId, Menu = y, Role = z };
            var dd = query.Cast<RoleMenu>().ToList();
            return dd;
        }
        public List<RoleMenu> GetAllRoleMenus()
        {
            var query = from x in _dbContext.connection.Query<RoleMenu>()
                        join y in _dbContext.connection.Query<Menu>() on x.MenuId equals y.Id into temp
                        from y in temp.DefaultIfEmpty()
                        join z in _dbContext.connection.Query<Role>() on x.RoleId equals z.Id into temp1
                        from z in temp1.DefaultIfEmpty()
                        select new RoleMenu() { MenuId = x.MenuId, RoleId = x.RoleId, Menu = y, Role = z };
            var dd = query.Cast<RoleMenu>().ToList();
            return dd;
        }
     
    }
}
