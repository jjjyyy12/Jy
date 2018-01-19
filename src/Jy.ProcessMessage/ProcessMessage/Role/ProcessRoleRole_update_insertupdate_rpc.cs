
using Jy.Domain.Message;
using Jy.Domain.IRepositories;
using Jy.Domain.Entities;
using Jy.Utility.Convert;
using Jy.DistributedLock;
using AutoMapper;
using Jy.IMessageQueue;
using Jy.QueueSerivce;
using Jy.Domain.Dtos;
using Jy.IRepositories;

namespace Jy.RabbitMQ.ProcessMessage
{
    /// <summary>
    ///  更新或新增角色
    /// </summary>
    public class ProcessRoleRole_update_insertupdate_rpc : IProcessMessage<role_update_insertupdate_rpc>
    {
        private readonly IRoleRepository _repository;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IQueueService _queueService;
        //队列
        public IQueueService QueueService { get { return _queueService; } }
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessRoleRole_update_insertupdate_rpc(IRoleRepository roleRepository, IRepositoryFactory repositoryFactory, IQueueService queueService)
        {
            _repository = roleRepository;
            _queueService = queueService;
            _repositoryFactory = repositoryFactory;
        }
        [DistributedLock("ProcessRole", 10)]
        public void ProcessMsg(role_update_insertupdate_rpc msg)
        {
            InsertUpdate(msg);
        }
        private void InsertUpdate(role_update_insertupdate_rpc msg)
        {
            var dto = ByteConvertHelper.Bytes2Object<RoleDto>(msg.MessageBodyByte);
            Role bodys = Mapper.Map <Role>(dto);
            Role retobj = null;
            lock (rpcLocker)
            {
                retobj = _repository.InsertOrUpdate(bodys);
            }
            if (retobj != null)
                msg.MessageBodyReturnByte = ByteConvertHelper.Object2Bytes(retobj.Id);

            var connList = _repositoryFactory.GetConnectionStrings();
            foreach(var connStr in connList)
            {
                role_update_others_normal otherobj = new role_update_others_normal(_queueService.ExchangeName, dto, connStr);
                _queueService.PublishTopic(otherobj);
            }
            
        }
    }
}
