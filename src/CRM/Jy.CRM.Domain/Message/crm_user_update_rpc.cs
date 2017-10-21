using Jy.CRM.Domain.Dtos;
using Jy.IMessageQueue;
using Jy.Utility.Convert;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.CRM.Domain.Message
{
    public class crm_user_update_rpc : MessageBase
    {
        public crm_user_update_rpc(string exchangeName, UserDto dto)
        {
            base.MessageBodyByte = ByteConvertHelper.Object2Bytes(dto);
            base.exchangeName = exchangeName;
            base.MessageRouter = this.GetType().Name;
        }
    }
}
