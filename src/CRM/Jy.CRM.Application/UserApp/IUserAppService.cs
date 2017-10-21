using System;
using System.Collections.Generic;
using System.Text;
using Jy.CRM.Domain.Dtos;
namespace Jy.CRM.Application.UserApp
{
    public interface IUserAppService
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        List<UserDto> GetAllList();
        /// <summary>
        /// 修改
        /// </summary>
        bool Update(UserDto dto);
        /// <summary>
        /// 新增,userId来自auth系统的userId
        /// </summary>
        bool Insert(UserDto dto);
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
    }
}
