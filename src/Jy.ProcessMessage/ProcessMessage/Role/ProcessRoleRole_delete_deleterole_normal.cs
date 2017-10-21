
using System;
using System.Collections.Generic;
using Jy.Domain.Message;
using Jy.Domain.IRepositories;
using Jy.Domain.Entities;
using Jy.Utility.Convert;
using Jy.DistributedLock;
using AutoMapper;
using Jy.IMessageQueue;
using Jy.IRepositories;
using Jy.QueueSerivce;

namespace Jy.RabbitMQ.ProcessMessage
{
    public class ProcessRoleRole_delete_deleterole_normal : IProcessMessage<role_delete_deleterole_normal>
    {
        private readonly IRoleRepository _repository;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IQueueService _queueService;

        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessRoleRole_delete_deleterole_normal(IRoleRepository roleRepository, IRepositoryFactory repositoryFactory, IQueueService queueService)
        {
            _repository = roleRepository;
            _queueService = queueService;
            _repositoryFactory = repositoryFactory;
        }
        [DistributedLock("ProcessRole", 10)]
        public void ProcessMsg(role_delete_deleterole_normal msg)
        {
             Delete(msg);
        }
      
        private void Delete(role_delete_deleterole_normal msg)
        {
            List<Guid> bodys = ByteConvertHelper.Bytes2Object<List<Guid>>(msg.MessageBodyByte);
            lock (normalLocker)
            {
                _repository.Delete(it => bodys.Contains(it.Id));
            }

            var connList = _repositoryFactory.GetConnectionStrings();
            foreach (var connStr in connList)
            {
                role_delete_others_normal otherobj = new role_delete_others_normal(_queueService.ExchangeName, bodys, connStr);
                _queueService.PublishTopic(otherobj);
            }
        }
      
    }
}
