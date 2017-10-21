
using System;
using Jy.Domain.Message;
using Jy.Domain.IRepositories;
using Jy.Domain.Entities;
using Jy.Utility.Convert;
using AutoMapper;
using Jy.DistributedLock;
using Jy.IMessageQueue;
using Jy.IRepositories;
using Jy.Domain.Dtos;

namespace Jy.RabbitMQ.ProcessMessage
{
    public class ProcessUser_update_insertupdate_rpc : IProcessMessage<user_update_insertupdate_rpc>
    {
        private readonly IRepositoryFactory _repository;
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessUser_update_insertupdate_rpc(IRepositoryFactory repository)
        {
            _repository = repository;
        }
        [DistributedLock("ProcessUser", 5)]
        public void ProcessMsg(user_update_insertupdate_rpc msg)
        {
            InsertUpdate(msg);
        }

        private void InsertUpdate(user_update_insertupdate_rpc msg)
        {
            User bodys = Mapper.Map<User>(ByteConvertHelper.Bytes2Object<UserDto>(msg.MessageBodyByte));
            User user = null;
            bool isAdd = default(Guid).Equals(bodys.Id);
            lock (rpcLocker)
            {
                if (!isAdd)
                {
                    user = _repository.CreateRepository<User,IUserRepository>(bodys.Id.ToString()).Update(bodys);
                }
                else
                {
                    bodys.Id = Guid.NewGuid();
                    user = _repository.CreateRepository<User, IUserRepository>(bodys.Id.ToString()).Insert(bodys);
                } 
            }
            if (user != null)
                msg.MessageBodyReturnByte = ByteConvertHelper.Object2Bytes(user.Id);
                
        }
    }
}
