using Jy.IRepositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.CRM.Domain.Entities
{
    public class Commodity : Entity
    {
        public string Name { get; set; }
        public int MaxNum { get; set; }
        public int MaxGouNum { get; set; }
        public decimal Price { get; set; }
        public string Des { get; set; }
        public string Url { get; set; }

        /// <summary>
        /// 订单
        /// </summary>
        public ICollection<SecKillOrder> SecKillOrders { get; set; }
    }
}
