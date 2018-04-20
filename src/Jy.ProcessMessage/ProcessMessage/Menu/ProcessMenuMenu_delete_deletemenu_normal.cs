
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
    /// <summary>
    /// 删除菜单
    /// </summary>
    public class ProcessMenuMenu_delete_deletemenu_normal : IProcessMessage<menu_delete_deletemenu_normal>
    {
        private readonly IMenuRepository _repository;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly Func<string, IRepositoryFactory> _repositoryAccessor;
        private readonly IQueueService _queueService;

        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessMenuMenu_delete_deletemenu_normal(IMenuRepository menuRepository, Func<string, IRepositoryFactory> repositoryAccessor, IQueueService queueService)
        {
            _repository = menuRepository;
            _queueService = queueService;
            _repositoryAccessor = repositoryAccessor;
            _repositoryFactory = _repositoryAccessor("EF");
        }
        [DistributedLock("ProcessMenu", 10)]
        public void ProcessMsg(menu_delete_deletemenu_normal msg)
        {
                Delete(msg);
        }
      
        private void Delete(menu_delete_deletemenu_normal msg)
        {
            List<Guid> bodys = ByteConvertHelper.Bytes2Object<List<Guid>>(msg.MessageBodyByte);
            lock (normalLocker)
            {
                _repository.Delete(it => bodys.Contains(it.Id));
            }

            var connList = _repositoryFactory.GetConnectionStrings();
            foreach (var connStr in connList)
            {
                menu_delete_others_normal otherobj = new menu_delete_others_normal(_queueService.ExchangeName, bodys, connStr);
                _queueService.PublishTopic(otherobj);
            }
        }
      
    }
}
