
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

namespace Jy.RabbitMQ.ProcessMessage
{
    /// <summary>
    /// 新增或修改菜单分库数据
    /// </summary>
    public class ProcessMenuMenu_update_others_normal : IProcessMessage<menu_update_others_normal>
    {
        private readonly IRepositoryFactory _repository;
        private readonly Func<string, IRepositoryFactory> _repositoryAccessor;
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessMenuMenu_update_others_normal(Func<string, IRepositoryFactory> repositoryAccessor)
        {
            _repositoryAccessor = repositoryAccessor;
            _repository = _repositoryAccessor("EF");
        }
        [DistributedLock("ProcessMenu", 10)]
        public void ProcessMsg(menu_update_others_normal msg)
        {
                InsertUpdate(msg);
        }
        private void InsertUpdate(menu_update_others_normal msg)
        {
            if (string.IsNullOrWhiteSpace(msg.CurrentDBStr))
                throw new Exception("IRepositoryFactory.CreateRepositoryByConnStr need menu ConnStr!");

            Menu bodys = Mapper.Map < Menu >(ByteConvertHelper.Bytes2Object<MenuDto>(msg.MessageBodyByte));
            Menu retobj = null;
            lock (rpcLocker)
            {
                retobj = _repository.CreateRepositoryByConnStr<Menu, IMenuRepository>(msg.CurrentDBStr).InsertOrUpdate(bodys);
            }
            if (retobj != null)
                msg.MessageBodyReturnByte = ByteConvertHelper.Object2Bytes(retobj.Id);
        }
    }
}
