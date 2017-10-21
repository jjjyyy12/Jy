using System;
using System.Collections.Generic;
using System.Text;
using Jy.IRepositories;
namespace Jy.CRM.Domain.Entities
{
    public class User : Entity
    {
        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 邮箱地址
        /// </summary>
        public string EMail { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string MobileNumber { get; set; }
        /// <summary>
        /// 通信地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        public ICollection<SecKillOrder> SecKillOrders { get; set; }

        public ICollection<UserAddress> UserAddresss { get; set; }
    }
}
