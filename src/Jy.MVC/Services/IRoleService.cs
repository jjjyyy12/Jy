using Jy.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.MVC.Services
{
    public interface IRoleService
    {
        Task<List<TreeCheckBoxModel>> GetMenuTreeData(Guid id);
        Task<Paged<Role>> GetListPaged(int startPage, int pageSize);
        Task<ReturnObj> Create(Role role);
        Task<ReturnObj> Edit(Role role);
        Task<ReturnObj> RoleMenu(RoleMenuModel rpm);
        Task<ReturnObj> DeleteMuti(string ids);
        Task<ReturnObj> Delete(Guid id);
        Task<Role> Get(Guid id);
    }
}
