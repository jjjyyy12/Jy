using Jy.Domain.Dtos;
using System;
using System.Collections.Generic;

namespace Jy.Application.UserApp
{
    public interface IUserAppService
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        //[Obsolete]
        //List<UserDto> GetAllList();

        /// <summary>
        /// 根据父级Id获取子级列表
        /// </summary>
        /// <param name="DepartmentId">DepartmentId</param>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        List<UserDto> GetChildrenByDepartment(Guid DepartmentId, int startPage, int pageSize, out int rowCount);

        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        bool InsertOrUpdate(UserDto dto,UserDto createUser);

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
        UserDto Get(Guid id);

        List<UserRoleDto> GetUserRoles(Guid id);

        void UpdateUserRoles(Guid id, List<Guid> roleIds);

        void BatchUpdateUserRoles(List<Guid> userIds, List<Guid> roleIds);


    }
}
