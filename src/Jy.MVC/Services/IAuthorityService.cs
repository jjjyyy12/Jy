using Jy.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.MVC.Services
{
    public interface IAuthorityService
    {
        Task<List<TreeCheckBoxModel>> GetRoleTreeData(Guid id);
        Task<List<TreeCheckBoxModel>> GetBatchRoleTreeData();
        Task<ReturnObj> BatchUserRole(BatchUserRoleModel rpm);
        Task<ReturnObj> UserRole(UserRoleModel rpm);
    }
}
