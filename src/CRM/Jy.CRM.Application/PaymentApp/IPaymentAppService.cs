﻿using Jy.CRM.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.CRM.Application.PaymentApp
{
    public interface IPaymentAppService
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        List<PaymentDto> GetAllList();
        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        bool InsertOrUpdate(PaymentDto dto);

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
        PaymentDto Get(Guid id);
    }
}
