using Jy.Domain.Entities;
using Jy.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.Domain.IRepositories
{
    public interface IRoleRepositoryRead : IRepositoryRead<Role>
    {
        List<RoleMenu> GetRoleMenus(Guid id);
        List<RoleMenu> GetUserRoleMenus(Guid id);
        List<RoleMenu> GetAllRoleMenus();
    }
}
