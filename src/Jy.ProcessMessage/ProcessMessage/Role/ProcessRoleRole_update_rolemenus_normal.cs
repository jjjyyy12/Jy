
using System.Collections.Generic;
using Jy.Domain.Message;
using Jy.Domain.IRepositories;
using Jy.Domain.Entities;
using Jy.Utility.Convert;
using Jy.DistributedLock;
using Jy.IMessageQueue;
using Jy.IRepositories;
using Jy.QueueSerivce;
using System;

namespace Jy.RabbitMQ.ProcessMessage
{
    /// <summary>
    /// 修改角色菜单对应关系
    /// </summary>
    public class ProcessRoleRole_update_rolemenus_normal : IProcessMessage<role_update_rolemenus_normal>
    {
        private readonly IRoleRepository _repository;
        private readonly Func<string, IRepositoryFactory> _repositoryAccessor;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IQueueService _queueService;

        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessRoleRole_update_rolemenus_normal(IRoleRepository roleRepository, Func<string, IRepositoryFactory> repositoryAccessor, IQueueService queueService)
        {
            _repository = roleRepository;
            _queueService = queueService;
            _repositoryAccessor = repositoryAccessor;
            _repositoryFactory = _repositoryAccessor("EF");
        }
        [DistributedLock("ProcessRoleMenus", 20)]
        public void ProcessMsg(role_update_rolemenus_normal msg)
        {
             UpdateRowMenus(msg);
        }
        private void UpdateRowMenus(role_update_rolemenus_normal msg)
        {
            RoleMenuMsg bodys = ByteConvertHelper.Bytes2Object<RoleMenuMsg>(msg.MessageBodyByte);
            List<RoleMenu> roleMenus = new List<RoleMenu>();
            bodys?.menuIds.ForEach(x => { roleMenus.Add(new RoleMenu() { RoleId = bodys.Id, MenuId = x }); });
            lock (normalLocker)
            {
                _repository.Execute(() =>
                {
                    _repository.RemoveRowMenus(bodys.Id);
                    _repository.UnitOfWork.SaveChange();
                    _repository.BatchAddRowMenus(roleMenus);
                    _repository.UnitOfWork.SaveChange();
                });
            }

            var connList = _repositoryFactory.GetConnectionStrings();
            foreach (var connStr in connList)
            {
                role_rolemenus_others_normal otherobj = new role_rolemenus_others_normal(_queueService.ExchangeName, bodys, connStr);
                _queueService.PublishTopic(otherobj);
            }
        }
    }
}
