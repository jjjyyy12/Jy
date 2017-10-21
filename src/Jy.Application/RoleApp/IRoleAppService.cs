using Jy.Domain.Dtos;
using System;
using System.Collections.Generic;

namespace Jy.Application.RoleApp
{
    public interface IRoleAppService
    {
    
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        List<RoleDto> GetAllList();

        /// <summary>
        /// 分页获取列表
        /// </summary>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        List<RoleDto> GetListPaged(int startPage, int pageSize, out int rowCount);


        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        bool InsertOrUpdate(RoleDto dto, UserDto moduser);

        /// <summary>
        /// 根据Id集合批量删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        void DeleteBatch(List<Guid> ids);

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">Id</param>
        void Delete(Guid id);

        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        RoleDto Get(Guid id);

        /// <summary>
        /// 获取角色所对应的功能列表
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<RoleMenuDto> GetRowMenus(Guid id);
        List<RoleMenuDto> GetAllRowMenus();
        List<string> GetUserRowMenusUrls(List<Guid> roleIds);
        //获取左侧菜单
        List<RoleMenuDto> GetRowMenuForLeftMenu(List<Guid> roleIds);
        void UpdateRowMenus(Guid id, List<Guid> menuIds);
 
    }
}
