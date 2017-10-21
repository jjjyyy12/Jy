using System;
using System.Collections.Generic;
using System.Text;
using Jy.IRepositories;
namespace Jy.CRM.Domain.Dtos
{
    public class PaymentDto : Entity
    {
        public Guid SecKillOrderId { get; set; }
        public string ChannelCode { get; set; }
        public string PaymentMode { get; set; }
        public string PaymentAccount { get; set; }
        public decimal PaymentAmount { get; set; }

    }
}
