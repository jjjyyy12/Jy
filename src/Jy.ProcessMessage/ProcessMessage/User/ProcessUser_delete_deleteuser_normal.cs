
using System;
using System.Collections.Generic;
using Jy.Domain.Message;
using Jy.Domain.IRepositories;
using Jy.Domain.Entities;
using Jy.Utility.Convert;
using Jy.DistributedLock;
using Jy.IMessageQueue;
using Jy.IRepositories;

namespace Jy.RabbitMQ.ProcessMessage
{
    /// <summary>
    /// 删除用户db数据
    /// </summary>
    public class ProcessUser_delete_deleteuser_normal : IProcessMessage<user_delete_deleteuser_normal>
    {
        private readonly IRepositoryFactory _repository;
        //一个接口多个实现
        private readonly Func<string, IRepositoryFactory> _repositoryAccessor;

        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessUser_delete_deleteuser_normal(Func<string, IRepositoryFactory> repositoryAccessor)
        {
            _repositoryAccessor = repositoryAccessor;
            _repository = _repositoryAccessor("EF");
        }
        [DistributedLock("ProcessUser", 5)]
        public void ProcessMsg(user_delete_deleteuser_normal msg)
        {
            Delete(msg);
        }
        
        private void Delete(user_delete_deleteuser_normal msg)
        {
            List<Guid> bodys = ByteConvertHelper.Bytes2Object<List<Guid>>(msg.MessageBodyByte);
            lock (normalLocker)
            {
                bodys.ForEach(item => {
                    _repository.CreateRepository<User, IUserRepository>(item.ToString()).Delete(item);//这里efcore自带级联删除
                });
            }
        }
    }
}
