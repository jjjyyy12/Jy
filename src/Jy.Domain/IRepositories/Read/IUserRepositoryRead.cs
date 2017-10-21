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
    public interface IUserRepositoryRead : IRepositoryRead<User>
    {
        UserIndex CheckUserIndex(string userName, string password);
        List<UserIndex> GetUserIndexList(Expression<Func<UserIndex, bool>> predicate);
        User GetUserInfo(Guid userId);
        /// <summary>
        /// 检查用户是存在
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>存在返回用户实体，否则返回NULL</returns>
        User CheckUser(string userName, string password);
        List<UserRole> GetUserRoles(Guid id);

    }
}
