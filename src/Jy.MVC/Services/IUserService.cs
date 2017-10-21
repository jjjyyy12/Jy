using Jy.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.MVC.Services
{
    public interface IUserService
    {
        Task<List<TreeModel>> GetTreeData();
        Task<Paged<User>> GetChildrenByParent(Guid departmentId, int startPage, int pageSize);
        Task<ReturnObj> Create(User user);
        Task<ReturnObj> Edit(User user);
        Task<ReturnObj> ResetPassword(ResetPasswordModel rpm);
        Task<ReturnObj> DeleteMuti(string ids);
        Task<ReturnObj> Delete(Guid id);
        Task<User> Get(Guid id);
    }
}
