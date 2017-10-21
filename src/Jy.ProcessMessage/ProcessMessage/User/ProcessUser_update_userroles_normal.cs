
using System;
using System.Collections.Generic;
using Jy.Domain.Message;
using Jy.Domain.IRepositories;
using Jy.Domain.Entities;
using Jy.Utility.Convert;
using AutoMapper;
using Jy.DistributedLock;
using Jy.IMessageQueue;
using Jy.IRepositories;

namespace Jy.RabbitMQ.ProcessMessage
{
    public class ProcessUser_update_userroles_normal : IProcessMessage<user_update_userroles_normal>
    {
        private readonly IRepositoryFactory _repository;
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessUser_update_userroles_normal(IRepositoryFactory repository)
        {
            _repository = repository;
        }
        [DistributedLock("ProcessUserRoles", 10)]
        public void ProcessMsg(user_update_userroles_normal msg)
        {
            UpdateUserRole(msg);
        }

        private void UpdateUserRole(user_update_userroles_normal msg)
        {
            UserRoleMsg bodys = ByteConvertHelper.Bytes2Object<UserRoleMsg>(msg.MessageBodyByte);
            List<UserRole> userRoles = new List<UserRole>();
            bodys?.userIds.ForEach(uid => { bodys?.roleIds.ForEach(rid => userRoles.Add(new UserRole() { UserId = uid, RoleId = rid })); });

            lock (normalLocker)
            {
                bodys.userIds.ForEach(id => {
                    var up = _repository.CreateRepository<User,IUserRepository>(id.ToString());
                    up.Execute(() =>
                    {
                        // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                        up.RemoveUserRoles(id, userRoles);
                        up.UnitOfWork.SaveChange();
                        up.BatchAddUserRoles(userRoles);
                        up.UnitOfWork.SaveChange();
                    });
                });
                
            }
        }
    }
}
