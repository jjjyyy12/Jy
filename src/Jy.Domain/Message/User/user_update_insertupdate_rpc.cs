using Jy.Domain.Dtos;
using Jy.IMessageQueue;
using Jy.Utility.Convert;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.Domain.Message
{
    public class user_update_insertupdate_rpc : MessageBase
    {
        public user_update_insertupdate_rpc(string exchangeName, UserDto dto)
        {
            base.MessageBodyByte = ByteConvertHelper.Object2Bytes(dto);
            base.exchangeName = exchangeName;
            base.MessageRouter = this.GetType().Name;
        }
    }
}
