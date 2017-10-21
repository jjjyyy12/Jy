
using System;
using System.Collections.Generic;
using Jy.Domain.Message;
using Jy.Domain.IRepositories;
using Jy.Utility.Convert;
using Jy.DistributedLock;
using Jy.IMessageQueue;

namespace Jy.RabbitMQ.ProcessMessage
{
    //delete userindex
    public class ProcessUser_delete_deleteuser_normal_2 : IProcessMessage<user_delete_deleteuser_normal>
    {
        private readonly IUserRepository _repository;//总库
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessUser_delete_deleteuser_normal_2(IUserRepository repository)
        {
            _repository = repository;
        }
        [DistributedLock("ProcessUserIndex", 5)]
        public void ProcessMsg(user_delete_deleteuser_normal msg)
        {
            Delete(msg);
        }
        
        private void Delete(user_delete_deleteuser_normal msg)
        {
            List<Guid> bodys = ByteConvertHelper.Bytes2Object<List<Guid>>(msg.MessageBodyByte);
            lock (normalLocker)
            {
                _repository.DeleteUserIndex(it => bodys.Contains(it.UserId));
            }
        }
    }
}
