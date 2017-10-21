using Jy.CRM.Domain.Message;
using Jy.IMessageQueue;
using Jy.Utility.Convert;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.CRM.Domain.Message
{
    public class crm_useraddress_update_normal : MessageBase
    {
        public crm_useraddress_update_normal(string exchangeName, UserAddressMsg dto)
        {
            MessageBodyByte = ByteConvertHelper.Object2Bytes(dto);
            base.exchangeName = exchangeName;
            MessageRouter = this.GetType().Name;
        }
    }
}
