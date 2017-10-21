using Jy.CRM.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.CRM.Application.SecKillOrderApp
{
    public interface ISecKillOrderAppService
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        List<SecKillOrderDto> GetAllList();
        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        bool InsertOrUpdate(SecKillOrderDto dto);

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
        SecKillOrderDto Get(Guid id);
    }
}
