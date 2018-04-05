using AutoMapper;
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
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Jy.Dapper.Repositories
{
    /// <summary>
    /// 用户管理仓储实现
    /// </summary>
    public class UserRepository : DapperRepositoryBase<User>, IUserRepository
    {
        protected readonly TransactedConnection dbContext;
        public UserRepository(IRepositoryContext context) : base(context)
        {
            dbContext = _dbContext;
        }
        public UserIndex EditUserIndex(User user,bool autoSave = true)
        {
            if (user == null) return new UserIndex();
            var ui = dbContext.connection.Query<UserIndex>(u => user.Id.Equals(u.UserId)).FirstOrDefault();
            if (ui == null) return null;
            var currui = Mapper.Map<UserIndex>(user);
                ui.DepartmentId = currui.DepartmentId;
                ui.Password = currui.Password;
                ui.UserName = currui.UserName;

            var statement = StatementFactory.Update<UserIndex>(Dialect.MySQL);
                dbContext.connection.Execute(statement, EntityToValue<UserIndex>(ui));
            if (autoSave)
                Save();

            return ui;
        }
        public UserIndex InsertUserIndex(User user, bool autoSave = true)
        {
            UserIndex ui = new UserIndex();
            ui = Mapper.Map<UserIndex>(user);

            var statement = StatementFactory.Insert<UserIndex>(Dialect.MySQL);
            _dbContext.connection.Execute(statement, EntityToValue<UserIndex>(ui));
            if (autoSave)
                Save();
            return ui;
        }
        public void DeleteUserIndex(Expression<Func<UserIndex, bool>> where, bool autoSave = true)
        {
            var items = dbContext.connection.Query<UserIndex>(where);
            var ids = items.Select(p => p.UserId).ToList();
            if (ids == null || ids.Count == 0) return;
            var statement = "delete from " + typeof(UserIndex).ToString() + " where UserId in (" + string.Join(",", ids.ToArray()) + ")";
            _dbContext.Execute(statement);
            if (autoSave)
                Save();
         
        }
        public void UpdateUserRoles(Guid id, List<UserRole> userRoles)
        {
     
            using (var transaction = dbContext.connection.BeginTransaction())
            {
                try
                {
                    var statement = "delete from " + typeof(UserRole).ToString() + " where UserId in (" + id + ")";
                    dbContext.Execute(statement);
                    Save();
                    
                    foreach(var entity in userRoles)
                    {
                        if (entity == null) continue;
                        dbContext.connection.Execute(StatementFactory.Insert<UserRole>(Dialect.MySQL), EntityToValue<UserRole>(entity));
                    }
                    Save();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }
        }

        public void BatchUpdateUserRoles(List<Guid> userIds, List<UserRole> userRoles)
        {
            using (var transaction = dbContext.connection.BeginTransaction())
            {
                try
                {
                    var statement = "delete from " + typeof(UserRole).ToString() + " where UserId in (" + string.Join(",", userIds.ToArray()) + ")";
                    dbContext.connection.Execute(statement);
                    
                    foreach (var entity in userRoles)
                    {
                        if (entity == null) continue;
                        dbContext.connection.Execute(StatementFactory.Insert<UserRole>(Dialect.MySQL), EntityToValue<UserRole>(entity));
                    }
                     
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }
        }
        public void RemoveUserRoles(Guid userId)
        {
            BatchRemoveUserRoles(new List<Guid>(1) { userId });
        }
        public void BatchRemoveUserRoles(List<Guid> userIds)
        {
            var statement = "delete from " + typeof(UserRole).ToString() + " where UserId in (" + string.Join(",", userIds.ToArray()) + ")";
            dbContext.Execute(statement);
            Save();
        }
        public void BatchAddUserRoles(List<UserRole> userRoles)
        {
            using (var transaction = dbContext.connection.BeginTransaction())
            {
                try
                {
                    foreach (var entity in userRoles)
                    {
                        if (entity == null) continue;
                        dbContext.connection.Execute(StatementFactory.Insert<UserRole>(Dialect.MySQL), EntityToValue<UserRole>(entity));
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
