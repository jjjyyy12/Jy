using Jy.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.MVC.Services
{
    public interface IDepartmentService
    {
        Task<List<TreeModel>> GetTreeData();
        Task<Paged<Department>> GetChildrenByParent(Guid parentId, int startPage, int pageSize);
        Task<ReturnObj> Create(Department dto);
        Task<ReturnObj> Edit(Department menu);
        Task<ReturnObj> DeleteMuti(string ids);
        Task<ReturnObj> Delete(Guid id);
        Task<Department> Get(Guid id);
    }
}
