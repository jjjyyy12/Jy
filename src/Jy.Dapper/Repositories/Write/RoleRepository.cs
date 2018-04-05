using Dapper;
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
    public class RoleRepository : DapperRepositoryBase<Role>, IRoleRepository
    {
        protected readonly TransactedConnection dbContext;
        public RoleRepository(IRepositoryContext dbcontext) : base(dbcontext)
        {
            dbContext = _dbContext;
        }
        public void UpdateRowMenus(Guid id, List<RoleMenu> roleMenus)
        {
            using (var transaction = dbContext.connection.BeginTransaction())
            {
                try
                {
                    var statement = "delete from " + typeof(RoleMenu).ToString() + " where RoleId in (" + id + ")";
                    dbContext.connection.Execute(statement);

                    foreach (var entity in roleMenus)
                    {
                        if (entity == null) continue;
                        dbContext.connection.Execute(StatementFactory.Insert<RoleMenu>(Dialect.MySQL), EntityToValue<RoleMenu>(entity));
                    }
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
            var statement = "delete from " + typeof(RoleMenu).ToString() + " where RoleId in (" + id + ")";
            dbContext.Execute(statement);
            Save();
        }
        public void BatchAddRowMenus(List<RoleMenu> roleMenus)
        {
            using (var transaction = dbContext.connection.BeginTransaction())
            {
                try
                {
                    foreach (var entity in roleMenus)
                    {
                        if (entity == null) continue;
                        dbContext.connection.Execute(StatementFactory.Insert<RoleMenu>(Dialect.MySQL), EntityToValue<RoleMenu>(entity));
                    }
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }
        }
    }
}
