
using Jy.Domain.Message;
using Jy.Domain.IRepositories;
using Jy.Domain.Entities;
using Jy.Utility.Convert;
using Jy.DistributedLock;
using AutoMapper;
using Jy.IMessageQueue;
using Jy.Domain.Dtos;
using Jy.IRepositories;
using Jy.QueueSerivce;
using System;

namespace Jy.RabbitMQ.ProcessMessage
{
    /// <summary>
    /// 新增修改部门信息
    /// </summary>
    public class ProcessDepartmentDepartment_update_insertupdate_rpc : IProcessMessage<department_update_insertupdate_rpc>
    {
        private readonly IDepartmentRepository _repository;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly Func<string, IRepositoryFactory> _repositoryAccessor;
        private readonly IQueueService _queueService;
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessDepartmentDepartment_update_insertupdate_rpc(IDepartmentRepository departmentRepository, Func<string, IRepositoryFactory> repositoryAccessor, IQueueService queueService)
        {
            _repository = departmentRepository;
            _queueService = queueService;
            _repositoryAccessor = repositoryAccessor;
            _repositoryFactory = _repositoryAccessor("EF");
        }
        [DistributedLock("ProcessDepartment", 10)]
        public void ProcessMsg(department_update_insertupdate_rpc msg)
        {
                InsertUpdate(msg);
        }
        private void InsertUpdate(department_update_insertupdate_rpc msg)
        {
            var dto = ByteConvertHelper.Bytes2Object<DepartmentDto>(msg.MessageBodyByte);
            Department bodys = Mapper.Map < Department >(dto);
            Department retobj = null;
            lock (rpcLocker)
            {
                retobj = _repository.InsertOrUpdate(bodys);
            }
            if (retobj != null)
                msg.MessageBodyReturnByte = ByteConvertHelper.Object2Bytes(retobj.Id);

            var connList = _repositoryFactory.GetConnectionStrings();
            foreach (var connStr in connList)
            {
                department_update_others_normal otherobj = new department_update_others_normal(_queueService.ExchangeName, dto, connStr);
                _queueService.PublishTopic(otherobj);
            }
        }
    }
}
