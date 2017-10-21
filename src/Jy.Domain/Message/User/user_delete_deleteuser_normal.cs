using Jy.IMessageQueue;
using Jy.Utility.Convert;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.Domain.Message
{
    public class user_delete_deleteuser_normal : MessageBase
    {
        public user_delete_deleteuser_normal(string exchangeName, List<Guid> ids)
        {
            base.MessageBodyByte = ByteConvertHelper.Object2Bytes(ids);
            base.exchangeName = exchangeName;
            base.MessageRouter = this.GetType().Name;
        }
    }
}
