
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
    public class ProcessDepartmentDepartment_update_others_normal : IProcessMessage<department_update_others_normal>
    {
        private readonly IRepositoryFactory _repository;
        private static readonly object rpcLocker = new object();
        private static readonly object normalLocker = new object();
        public ProcessDepartmentDepartment_update_others_normal(IRepositoryFactory departmentRepository)
        {
            _repository = departmentRepository;
        }
        [DistributedLock("ProcessDepartment", 10)]
        public void ProcessMsg(department_update_others_normal msg)
        {
            InsertUpdate(msg);
        }
        private void InsertUpdate(department_update_others_normal msg)
        {
            Department bodys = Mapper.Map < Department >(ByteConvertHelper.Bytes2Object<DepartmentDto>(msg.MessageBodyByte));
            Department retobj = null;
            lock (rpcLocker)
            {
                retobj = _repository.CreateRepositoryByConnStr<Department, IDepartmentRepository>(msg.CurrentDBStr).InsertOrUpdate(bodys);
            }
            if (retobj != null)
                msg.MessageBodyReturnByte = ByteConvertHelper.Object2Bytes(retobj.Id);
        }
    }
}
