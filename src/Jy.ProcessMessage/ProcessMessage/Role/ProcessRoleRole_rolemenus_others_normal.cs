
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
    //更新其他分库的角色基础信息
    public class ProcessRoleRole_rolemenus_others_normal : IProcessMessage<role_rolemenus_others_normal>
    {
        private readonly IRepositoryFactory _repository;
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessRoleRole_rolemenus_others_normal(IRepositoryFactory roleRepository)
        {
            _repository = roleRepository;
        }
        [DistributedLock("ProcessRoleMenus", 20)]
        public void ProcessMsg(role_rolemenus_others_normal msg)
        {
             UpdateRowMenus(msg);
        }
        private void UpdateRowMenus(role_rolemenus_others_normal msg)
        {
            if (string.IsNullOrWhiteSpace(msg.CurrentDBStr))
                throw new Exception("IRepositoryFactory.CreateRepositoryByConnStr need role ConnStr!");

            RoleMenuMsg bodys = ByteConvertHelper.Bytes2Object<RoleMenuMsg>(msg.MessageBodyByte);
            List<RoleMenu> roleMenus = new List<RoleMenu>();
            bodys?.menuIds.ForEach(x => { roleMenus.Add(new RoleMenu() { RoleId = bodys.Id, MenuId = x }); });
            lock (normalLocker)
            {
                var repository = _repository.CreateRepositoryByConnStr<Role, IRoleRepository>(msg.CurrentDBStr);
                repository.Execute(() =>
                {
                    repository.RemoveRowMenus(bodys.Id);
                    repository.UnitOfWork.SaveChange();
                    repository.BatchAddRowMenus(roleMenus);
                    repository.UnitOfWork.SaveChange();
                });
            }
        }
    }
}
