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
    public class RoleRepository : JyRepositoryBase<Role, JyDbContext>, IRoleRepository
    {
        public RoleRepository(JyDbContext dbcontext) : base(dbcontext, dbcontext)
        {

        }
        public void UpdateRowMenus(Guid id, List<RoleMenu> roleMenus)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.RoleMenus.RemoveRange(_dbContext.Set<RoleMenu>().Where(it => it.RoleId == id));
                    _dbContext.SaveChanges();
                    _dbContext.RoleMenus.AddRange(roleMenus);
                    _dbContext.SaveChanges();
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
            _dbContext.RoleMenus.RemoveRange(_dbContext.Set<RoleMenu>().Where(it => it.RoleId == id));
        }
        public void BatchAddRowMenus(List<RoleMenu> roleMenus)
        {
            _dbContext.RoleMenus.AddRange(roleMenus);
        }
    }
}
