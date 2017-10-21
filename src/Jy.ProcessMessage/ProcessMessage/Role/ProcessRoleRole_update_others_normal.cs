﻿
using System;
using Jy.Domain.Message;
using Jy.Domain.IRepositories;
using Jy.Domain.Entities;
using Jy.Utility.Convert;
using Jy.DistributedLock;
using AutoMapper;
using Jy.IMessageQueue;
using Jy.IRepositories;
using Jy.Domain.Dtos;

namespace Jy.RabbitMQ.ProcessMessage
{
    //更新其他分库的角色基础信息
    public class ProcessRoleRole_update_others_normal : IProcessMessage<role_update_others_normal>
    {
        private readonly IRepositoryFactory _repository;
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessRoleRole_update_others_normal(IRepositoryFactory roleRepository)
        {
            _repository = roleRepository;
        }
        [DistributedLock("ProcessRole", 10)]
        public void ProcessMsg(role_update_others_normal msg)
        {
             InsertUpdate(msg);
        }
        private void InsertUpdate(role_update_others_normal msg)
        {
            if (string.IsNullOrWhiteSpace(msg.CurrentDBStr))
                throw new Exception("IRepositoryFactory.CreateRepositoryByConnStr need role ConnStr!"); 

            Role bodys = Mapper.Map<Role>(ByteConvertHelper.Bytes2Object<RoleDto>(msg.MessageBodyByte));
            Role retobj = null;
            lock (rpcLocker)
            {//更新分库的基础信息role
                retobj =  _repository.CreateRepositoryByConnStr<Role, IRoleRepository>(msg.CurrentDBStr).InsertOrUpdate(bodys);
            }
            if (retobj != null)
                msg.MessageBodyReturnByte = ByteConvertHelper.Object2Bytes(retobj.Id);
        }
    }
}
