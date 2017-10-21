using Jy.IMessageQueue;
using Jy.Utility.Convert;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jy.Domain.Message
{
    public class role_delete_others_normal : MessageBase
    {
        private string _currentDBStr;
        public string CurrentDBStr { get; set; }
        public role_delete_others_normal(string exchangeName, List<Guid> ids, string currentDBStr)
        {
            base.MessageBodyByte = ByteConvertHelper.Object2Bytes(ids);
            base.exchangeName = exchangeName;
            base.MessageRouter = this.GetType().Name;
            _currentDBStr = currentDBStr;
        }
    }
}
