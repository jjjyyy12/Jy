using Jy.IMessageQueue;
using Jy.Utility.Convert;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.Domain.Message
{
    public class menu_delete_deletemenu_normal :MessageBase
    {
        public menu_delete_deletemenu_normal(string exchangeName, List<Guid> ids)
        {
            base.MessageBodyByte = ByteConvertHelper.Object2Bytes(ids);
            base.exchangeName = exchangeName;
            base.MessageRouter = this.GetType().Name;
        }
    }
}
