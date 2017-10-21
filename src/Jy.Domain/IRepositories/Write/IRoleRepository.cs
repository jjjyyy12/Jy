using Jy.Domain.Entities;
using Jy.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.Domain.IRepositories
{
    public interface IRoleRepository : IRepository<Role>
    {
        void UpdateRowMenus(Guid id, List<RoleMenu> roleMenus);

        void RemoveRowMenus(Guid id);

        void BatchAddRowMenus(List<RoleMenu> roleMenus);
    }
}
