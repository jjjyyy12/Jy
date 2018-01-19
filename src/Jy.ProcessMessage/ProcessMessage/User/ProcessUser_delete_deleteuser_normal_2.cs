
using System;
using System.Collections.Generic;
using Jy.Domain.Message;
using Jy.Domain.IRepositories;
using Jy.Utility.Convert;
using Jy.DistributedLock;
using Jy.IMessageQueue;
using Jy.IIndex;
using Jy.AuthAdmin.SolrIndex;
using Jy.Domain.Entities;
using Microsoft.Extensions.Options;
using Jy.Domain.IIndex;

namespace Jy.RabbitMQ.ProcessMessage
{

    /// <summary>
    /// 删除用户索引  delete userindex
    /// </summary>
    public class ProcessUser_delete_deleteuser_normal_2 : IProcessMessage<user_delete_deleteuser_normal>
    {
        private readonly IUserRepository _repository;//总库
        private readonly IIndexFactory _indexFactory;
        private readonly IOptionsSnapshot<SIndexSettings> _SIndexSettings;
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessUser_delete_deleteuser_normal_2(IUserRepository repository, IOptionsSnapshot<SIndexSettings> SIndexSettings)
        {
            _repository = repository;
            _SIndexSettings = SIndexSettings;
            _indexFactory = new IndexFactory<UserIndexs>(_SIndexSettings);
        }
        [DistributedLock("ProcessUserIndex", 5)]
        public void ProcessMsg(user_delete_deleteuser_normal msg)
        {
            Delete(msg);
        }
        
        private void Delete(user_delete_deleteuser_normal msg)
        {
            List<Guid> bodys = ByteConvertHelper.Bytes2Object<List<Guid>>(msg.MessageBodyByte);
            lock (normalLocker)
            {
                _repository.DeleteUserIndex(it => bodys.Contains(it.UserId));
                DeleteIndex(bodys);
            }
        }
        private void DeleteIndex(List<Guid> ids)
        {
            ids.ForEach(id => {
                if (!default(Guid).Equals(id))
                {
                    var objindex = _indexFactory.CreateIndex<UserIndexs, IUserIndexsIndex>(id.ToString(), "authcore1");
                    objindex.Delete(id);
                }
            });
           
        }
    }
}
