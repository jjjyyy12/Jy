using Jy.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.MVC.Services
{
    public interface IMenuService
    {
        Task<List<TreeModel>> GetMenuTreeData();
        Task<Paged<Menu>> GetMenusByParent(Guid parentId, int startPage, int pageSize);
        Task<ReturnObj> Create(Menu menu);
        Task<ReturnObj> Edit(Menu menu);
        Task<ReturnObj> DeleteMuti(string ids);
        Task<ReturnObj> Delete(Guid id);
        Task<Menu> Get(Guid id);
    }
}
