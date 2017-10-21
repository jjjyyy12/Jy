
using System;
using System.Collections.Generic;
using Jy.Domain.Message;
using Jy.Domain.IRepositories;
using Jy.Domain.Entities;
using Jy.Utility.Convert;
using Jy.DistributedLock;
using Jy.IMessageQueue;
using Jy.IRepositories;

namespace Jy.RabbitMQ.ProcessMessage
{
    //更新其他分库的角色基础信息
    public class ProcessRoleRole_delete_others_normal : IProcessMessage<role_delete_others_normal>
    {
        private readonly IRepositoryFactory _repository;

        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessRoleRole_delete_others_normal(IRepositoryFactory roleRepository)
        {
            _repository = roleRepository;
        }
        [DistributedLock("ProcessRole", 10)]
        public void ProcessMsg(role_delete_others_normal msg)
        {
             Delete(msg);
        }
      
        private void Delete(role_delete_others_normal msg)
        {
            if (string.IsNullOrWhiteSpace(msg.CurrentDBStr))
                throw new Exception("IRepositoryFactory.CreateRepositoryByConnStr need role ConnStr!");

            List<Guid> bodys = ByteConvertHelper.Bytes2Object<List<Guid>>(msg.MessageBodyByte);
            lock (normalLocker)
            {
                _repository.CreateRepositoryByConnStr<Role, IRoleRepository>(msg.CurrentDBStr).Delete(it => bodys.Contains(it.Id));
            }
        }
      
    }
}
