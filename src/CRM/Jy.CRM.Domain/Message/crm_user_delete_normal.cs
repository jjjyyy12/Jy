using Jy.IMessageQueue;
using Jy.Utility.Convert;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.CRM.Domain.Message
{
    public class crm_user_delete_normal : MessageBase
    {
        public crm_user_delete_normal(string exchangeName, List<Guid> ids)
        {
            base.MessageBodyByte = ByteConvertHelper.Object2Bytes(ids);
            base.exchangeName = exchangeName;
            base.MessageRouter = this.GetType().Name;
        }
    }
}
