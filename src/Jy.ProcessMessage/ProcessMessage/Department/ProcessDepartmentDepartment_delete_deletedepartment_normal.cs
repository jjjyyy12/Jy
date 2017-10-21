
using System;
using System.Collections.Generic;
using Jy.Domain.Message;
using Jy.Domain.IRepositories;
using Jy.Utility.Convert;
using Jy.DistributedLock;
using Jy.IMessageQueue;
using Jy.IRepositories;
using Jy.QueueSerivce;

namespace Jy.RabbitMQ.ProcessMessage
{
    public class ProcessDepartmentDepartment_delete_deletedepartment_normal : IProcessMessage<department_delete_deletedepartment_normal>
    {
        private readonly IDepartmentRepository _repository;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IQueueService _queueService;
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessDepartmentDepartment_delete_deletedepartment_normal(IDepartmentRepository departmentRepository, IRepositoryFactory repositoryFactory, IQueueService queueService)
        {
            _repository = departmentRepository;
            _queueService = queueService;
            _repositoryFactory = repositoryFactory;
        }
        [DistributedLock("ProcessDepartment", 10)]
        public void ProcessMsg(department_delete_deletedepartment_normal msg)
        {
             Delete(msg);
        }
      
        private void Delete(department_delete_deletedepartment_normal msg)
        {
            List<Guid> bodys = ByteConvertHelper.Bytes2Object<List<Guid>>(msg.MessageBodyByte);
            lock (normalLocker)
            {
                _repository.Delete(it => bodys.Contains(it.Id));
            }
            var connList = _repositoryFactory.GetConnectionStrings();
            foreach (var connStr in connList)
            {
                department_delete_others_normal otherobj = new department_delete_others_normal(_queueService.ExchangeName, bodys, connStr);
                _queueService.PublishTopic(otherobj);
            }
        }
      
    }
}
