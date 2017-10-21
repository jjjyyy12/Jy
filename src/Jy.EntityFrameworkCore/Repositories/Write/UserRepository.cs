using AutoMapper;
using Jy.Domain.Entities;
using Jy.Domain.IRepositories;
using Jy.EntityFramewordCoreBase.Connection;
using Jy.EntityFramewordCoreBase.Repositories;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Jy.EntityFrameworkCore.Repositories
{
    /// <summary>
    /// 用户管理仓储实现
    /// </summary>
    public class UserRepository : JyRepositoryBase<User, JyDbContext>, IUserRepository
    {
        public UserRepository(JyDbContext dbcontext) : base(dbcontext, dbcontext)
        {
        }
        public UserIndex EditUserIndex(User user,bool autoSave = true)
        {
            var ui = _dbContext.Set<UserIndex>().Find(user.Id);
            if (ui == null) return null;
            var currui = Mapper.Map<UserIndex>(user);
                ui.DepartmentId = currui.DepartmentId;
                ui.Password = currui.Password;
                ui.UserName = currui.UserName;

            _dbContext.Set<UserIndex>().Update(ui);
            if (autoSave)
                Save();
            return ui;
        }
        public UserIndex InsertUserIndex(User user, bool autoSave = true)
        {
            UserIndex ui = new UserIndex();
            ui = Mapper.Map<UserIndex>(user);
             
            _dbContext.Set<UserIndex>().Add(ui);
            if (autoSave)
                Save();
            return ui;
        }
        public void DeleteUserIndex(Expression<Func<UserIndex, bool>> where, bool autoSave = true)
        {
            _dbContext.Set<UserIndex>().Where(where).ToList().ForEach(it => _dbContext.Set<UserIndex>().Remove(it));
            if (autoSave)
                Save();
        }
        public void UpdateUserRoles(Guid id, List<UserRole> userRoles)
        {
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    _dbContext.UserRoles.RemoveRange(_dbContext.Set<UserRole>().Where(it => it.UserId == id));
                    _dbContext.SaveChanges();
                    _dbContext.UserRoles.AddRange(userRoles);
                    _dbContext.SaveChanges();
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
            using (var transaction = _dbContext.Database.BeginTransaction())
            {
                try
                {
                    userIds.ForEach(x => _dbContext.UserRoles.RemoveRange(_dbContext.Set<UserRole>().Where(it => it.UserId == x)));
                    _dbContext.SaveChanges();
                    _dbContext.UserRoles.AddRange(userRoles);
                    _dbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                }
            }
        }
        public void RemoveUserRoles(Guid userId, List<UserRole> userRoles)
        {
            _dbContext.UserRoles.RemoveRange(_dbContext.Set<UserRole>().Where(it => it.UserId == userId));
        }
        public void BatchRemoveUserRoles(List<Guid> userIds, List<UserRole> userRoles)
        {
            userIds.ForEach(x => _dbContext.UserRoles.RemoveRange(_dbContext.Set<UserRole>().Where(it => it.UserId == x)));
        }
        public void BatchAddUserRoles(List<UserRole> userRoles)
        {
            _dbContext.UserRoles.AddRange(userRoles);
        }
    }
}
