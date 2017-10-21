using Jy.Domain.Entities;
using Jy.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Jy.Domain.IRepositories
{
    /// <summary>
    /// 用户管理仓储接口
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {

        UserIndex InsertUserIndex(User user, bool autoSave = true);
        UserIndex EditUserIndex(User user, bool autoSave = true);
        void DeleteUserIndex(Expression<Func<UserIndex, bool>> where, bool autoSave = true);
        void UpdateUserRoles(Guid id, List<UserRole> userRoles);

        void BatchUpdateUserRoles(List<Guid> userIds, List<UserRole> userRoles);

        void BatchRemoveUserRoles(List<Guid> userIds, List<UserRole> userRoles);
        void RemoveUserRoles(Guid userId, List<UserRole> userRoles);

        void BatchAddUserRoles(List<UserRole> userRoles);
         

    }
}
