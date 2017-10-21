
using System;
using System.Collections.Generic;
using Jy.Domain.Message;
using Jy.Utility.Convert;
using Jy.DistributedLock;
using Jy.IMessageQueue;
using Jy.IRepositories;
using Jy.QueueSerivce;
using Jy.Domain.Entities;
using Jy.Domain.IRepositories;

namespace Jy.RabbitMQ.ProcessMessage
{
    public class ProcessDepartmentDepartment_delete_others_normal : IProcessMessage<department_delete_others_normal>
    {
        private readonly IRepositoryFactory _repository;
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessDepartmentDepartment_delete_others_normal(IRepositoryFactory departmentRepository, IRepositoryFactory repositoryFactory, IQueueService queueService)
        {
            _repository = departmentRepository;
        }
        [DistributedLock("ProcessDepartment", 10)]
        public void ProcessMsg(department_delete_others_normal msg)
        {
             Delete(msg);
        }
      
        private void Delete(department_delete_others_normal msg)
        {
            List<Guid> bodys = ByteConvertHelper.Bytes2Object<List<Guid>>(msg.MessageBodyByte);
            lock (normalLocker)
            {
                _repository.CreateRepositoryByConnStr<Department, IDepartmentRepository>(msg.CurrentDBStr).Delete(it => bodys.Contains(it.Id));
            }
        }
      
    }
}
