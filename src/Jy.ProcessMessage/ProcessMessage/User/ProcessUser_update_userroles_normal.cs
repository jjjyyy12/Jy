
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
using Jy.IIndex;
using Microsoft.Extensions.Options;
using Jy.AuthAdmin.SolrIndex;
using Jy.Domain.IIndex;
using System.Text;

namespace Jy.RabbitMQ.ProcessMessage
{
    /// <summary>
    /// 更新用户角色
    /// </summary>
    public class ProcessUser_update_userroles_normal : IProcessMessage<user_update_userroles_normal>
    {
        private readonly IRepositoryFactory _repository;
        //一个接口多个实现
        private readonly Func<string, IRepositoryFactory> _repositoryAccessor;
        private readonly IIndexFactory _indexFactory;
        private readonly IIndexReadFactory _indexReadFactory;
        private readonly IOptionsSnapshot<SIndexSettings> _SIndexSettings;
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessUser_update_userroles_normal(Func<string, IRepositoryFactory> repositoryAccessor, IOptionsSnapshot<SIndexSettings> SIndexSettings)
        {
            _repositoryAccessor = repositoryAccessor;
            _repository = _repositoryAccessor("EF");
            _SIndexSettings = SIndexSettings;
            _indexReadFactory = new IndexReadFactory<UserIndexs>(_SIndexSettings);
            _indexFactory = new IndexFactory<UserIndexs>(_SIndexSettings);
        }
        [DistributedLock("ProcessUserRoles", 10)]
        public void ProcessMsg(user_update_userroles_normal msg)
        {
            UpdateUserRole(msg);
        }

        private void UpdateUserRole(user_update_userroles_normal msg)
        {
            UserRoleMsg bodys = ByteConvertHelper.Bytes2Object<UserRoleMsg>(msg.MessageBodyByte);
            if (bodys == null) return;
            List<UserRole> userRoles = new List<UserRole>();
            Dictionary<Guid,string> userRoleStrs = new Dictionary<Guid, string>(bodys.userIds.Count);
            bodys.userIds.ForEach(uid => {
                StringBuilder roleStrs = new StringBuilder();
                bodys?.roleIds.ForEach(rid => { userRoles.Add(new UserRole() { UserId = uid, RoleId = rid }); roleStrs.Append( rid).Append(","); });
                roleStrs.Remove(roleStrs.Length - 1, 1);
                userRoleStrs.Add(uid, roleStrs.ToString());
            });

            lock (normalLocker)
            {
                bodys.userIds.ForEach(id => {
                    var up = _repository.CreateRepository<User,IUserRepository>(id.ToString());
                    up.Execute(() =>
                    {
                        //get from indexs
                        var readindex = _indexReadFactory.CreateIndex<UserIndexs, IUserIndexsIndexRead>(id.ToString(), "authcore1");
                        var userindex = readindex.FirstOrDefault(new List<KeyValuePair<string, string>>(1) { new KeyValuePair<string, string>("id", id.ToString())  });
                        if (userindex != null)
                        {
                            userindex.roles = userRoleStrs[id];
                            //update indexs
                            var objindex = _indexFactory.CreateIndex<UserIndexs, IUserIndexsIndex>(id.ToString(), "authcore1");
                            objindex.Insert(userindex);
                        }
                        
                        // Achieving atomicity between original catalog database operation and the IntegrationEventLog thanks to a local transaction
                        up.RemoveUserRoles(id);
                        up.UnitOfWork.SaveChange();
                        up.BatchAddUserRoles(userRoles);
                        up.UnitOfWork.SaveChange();
                    });
                });
                
            }
        }
    }
}
