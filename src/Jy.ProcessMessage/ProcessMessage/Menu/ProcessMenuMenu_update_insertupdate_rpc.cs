
using System;
using System.Collections.Generic;
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

namespace Jy.RabbitMQ.ProcessMessage
{
    /// <summary>
    /// 新增或修改菜单
    /// </summary>
    public class ProcessMenuMenu_update_insertupdate_rpc : IProcessMessage<menu_update_insertupdate_rpc>
    {
        private readonly IMenuRepository _repository;
        private readonly IRepositoryFactory _repositoryFactory;
        private readonly IQueueService _queueService;
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessMenuMenu_update_insertupdate_rpc(IMenuRepository menuRepository, IRepositoryFactory repositoryFactory, IQueueService queueService)
        {
            _repository = menuRepository;
            _queueService = queueService;
            _repositoryFactory = repositoryFactory;
        }
        [DistributedLock("ProcessMenu", 10)]
        public void ProcessMsg(menu_update_insertupdate_rpc msg)
        {
            InsertUpdate(msg);
        }
        private void InsertUpdate(menu_update_insertupdate_rpc msg)
        {
            var dto = ByteConvertHelper.Bytes2Object<MenuDto>(msg.MessageBodyByte);
            Menu bodys = Mapper.Map < Menu >(dto);
            Menu retobj = null;
            lock (rpcLocker)
            {
                retobj = _repository.InsertOrUpdate(bodys);
            }
            if (retobj != null)
                msg.MessageBodyReturnByte = ByteConvertHelper.Object2Bytes(retobj.Id);

            var connList = _repositoryFactory.GetConnectionStrings();
            foreach (var connStr in connList)
            {
                menu_update_others_normal otherobj = new menu_update_others_normal(_queueService.ExchangeName, dto, connStr);
                _queueService.PublishTopic(otherobj);
            }
        }
    }
}
