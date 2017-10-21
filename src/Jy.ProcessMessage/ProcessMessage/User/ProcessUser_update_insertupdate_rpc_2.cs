using System;
using Jy.Domain.Message;
using Jy.Domain.IRepositories;
using Jy.Domain.Entities;
using Jy.Utility.Convert;
using AutoMapper;
using Jy.DistributedLock;
using Jy.IMessageQueue;

namespace Jy.RabbitMQ.ProcessMessage
{
    //update userindex
    public class ProcessUser_update_insertupdate_rpc_2 : IProcessMessage<user_update_insertupdate_rpc>
    {
        private readonly IUserRepository _repository;//总库
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessUser_update_insertupdate_rpc_2(IUserRepository repository)
        {
            _repository = repository;
        }
        [DistributedLock("ProcessUserIndex", 5)]
        public void ProcessMsg(user_update_insertupdate_rpc msg)
        {
            InsertUpdate(msg);
        }

        private void InsertUpdate(user_update_insertupdate_rpc msg)
        {
            User bodys = Mapper.Map<User>(ByteConvertHelper.Bytes2Object<User>(msg.MessageBodyByte));
            UserIndex user = null;
            bool isAdd = default(Guid).Equals(bodys.Id);
            lock (rpcLocker)
            {
                if (!isAdd)
                {
                    user = _repository.EditUserIndex(bodys);
                }
                else
                {
                    var userId = ByteConvertHelper.Bytes2Object<Guid>(msg.MessageBodyReturnByte);
                    var newUser = _repository.Get(userId);
                    user = _repository.InsertUserIndex(newUser);
                }
            }
            if (user != null)
                msg.MessageBodyReturnByte = ByteConvertHelper.Object2Bytes(user.UserId);

        }
    }
}
