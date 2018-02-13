using Jy.Domain.Entities;
using Jy.Domain.IRepositories;
using Jy.EntityFramewordCoreBase.Repositories;
using Jy.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq; 
 
namespace Jy.EntityFrameworkCore.Repositories
{
    public class RoleRepository : EntityFrameworkRepositoryBase<Role>, IRoleRepository
    {
        protected readonly JyDbContext dbContext;
        public RoleRepository(IRepositoryContext dbcontext) : base(dbcontext)
        {
            dbContext = (JyDbContext)_dbContext;
        }
        public void UpdateRowMenus(Guid id, List<RoleMenu> roleMenus)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    dbContext.RoleMenus.RemoveRange(dbContext.Set<RoleMenu>().Where(it => it.RoleId == id));
                    dbContext.SaveChanges();
                    dbContext.RoleMenus.AddRange(roleMenus);
                    dbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }
        }

        public void RemoveRowMenus(Guid id)
        {
            dbContext.RoleMenus.RemoveRange(dbContext.Set<RoleMenu>().Where(it => it.RoleId == id));
        }
        public void BatchAddRowMenus(List<RoleMenu> roleMenus)
        {
            dbContext.RoleMenus.AddRange(roleMenus);
        }
    }
}
