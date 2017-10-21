using System;
using System.Collections.Generic;
using System.Text;
using Jy.IRepositories;
namespace Jy.CRM.Domain.Entities
{
    public class SecKillOrder : Entity
    {


        /// <summary>
        /// 支付超时时间  默认下单后30分钟
        /// </summary>
        public DateTime PayOutTime { get; set; }

        /// <summary>
        /// 抢购订单时间
        /// </summary>
        public DateTime CreatTime { get; set; }
        /// <summary>
        /// 支付订单时间
        /// </summary>
        public DateTime PayTime { get; set; }
        /// <summary>
        /// 商品编号
        /// </summary>
        public Guid CommodityId { get; set; }

        /// <summary>
        /// 购买商品数量
        /// </summary>
        public int Num { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }
        /// <summary>
        /// 商品信息
        /// </summary>
        public Commodity Commodity { get; set; }

        /// <summary>
        /// 支付信息
        /// </summary>
        public ICollection<Payment> Payments { get; set; }

        /// <summary>
        /// 订单状态 1:正在排队抢购 2：抢购成功 3：抢购失败  EmOrderStatus
        /// </summary>
        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        /// <summary>
        /// 地址信息
        /// </summary>
        public int AddressId { get; set; }
        public Address Address { get; set; }
    }
}
