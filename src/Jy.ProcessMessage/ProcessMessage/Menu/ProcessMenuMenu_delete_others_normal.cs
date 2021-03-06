﻿
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

namespace Jy.RabbitMQ.ProcessMessage
{
    /// <summary>
    /// 删除分库中菜单数据
    /// </summary>
    public class ProcessMenuMenu_delete_others_normal : IProcessMessage<menu_delete_others_normal>
    {
        private readonly IRepositoryFactory _repository;
        private readonly Func<string, IRepositoryFactory> _repositoryAccessor;
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessMenuMenu_delete_others_normal(Func<string, IRepositoryFactory> repositoryAccessor)
        {
            _repositoryAccessor = repositoryAccessor;
            _repository = _repositoryAccessor("EF");
        }
        [DistributedLock("ProcessMenu", 10)]
        public void ProcessMsg(menu_delete_others_normal msg)
        {
                Delete(msg);
        }
      
        private void Delete(menu_delete_others_normal msg)
        {
            if (string.IsNullOrWhiteSpace(msg.CurrentDBStr))
                throw new Exception("IRepositoryFactory.CreateRepositoryByConnStr need menu ConnStr!");

            List<Guid> bodys = ByteConvertHelper.Bytes2Object<List<Guid>>(msg.MessageBodyByte);
            lock (normalLocker)
            {
                _repository.CreateRepositoryByConnStr<Menu, IMenuRepository>(msg.CurrentDBStr).Delete(it => bodys.Contains(it.Id));
            }
        }
      
    }
}
