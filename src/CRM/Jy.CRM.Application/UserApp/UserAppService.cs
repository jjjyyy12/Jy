using System;
using System.Collections.Generic;
using Jy.CRM.Domain.Dtos;
using Jy.CRM.Domain.Entities;
using Jy.CRM.Domain.IRepositories;
using AutoMapper;
using Jy.Utility.Convert;
using Jy.Utility.Paged;
using Jy.CRM.Domain.Message;
using Jy.ILog;
using Jy.Utility.Const;
using Jy.CacheService;
using Jy.QueueSerivce;
using Jy.IRepositories;

namespace Jy.CRM.Application.UserApp
{
    /// <summary>
    /// 用户管理服务
    /// </summary>
    public class UserAppService : IUserAppService
    {
        //用户管理仓储接口读
        private readonly IUserRepositoryRead _repositoryRead;
        //时时根据userid切换库
        private readonly IRepositoryReadFactory _repositoryReadFactory;
        private readonly ICacheService _cacheService;
         //缓存 
        public ICacheService CacheService {  get { return _cacheService; } }

        private readonly IQueueService _queueService;
        //队列
        public IQueueService QueueService {  get { return _queueService; } }
       
        private ILogger _logger;

        private PagedHelper _pagedHelper;
        /// <summary>
        /// 构造函数 实现依赖注入
        /// </summary>
        public UserAppService( IUserRepositoryRead  repositoryRead, IRepositoryReadFactory repositoryReadFactory, ICacheService cacheService, IQueueService queueService, ILogger logger, PagedHelper pagedHelper)
        {
            _repositoryRead = repositoryRead;
            _repositoryReadFactory = repositoryReadFactory;
            _cacheService = cacheService;
            _queueService = queueService;
            _logger = logger;
            _pagedHelper = pagedHelper;
        }
      
        public void Delete(Guid id)
        {
            List<Guid> ids = new List<Guid>(); ids.Add(id);
            DeleteCache(ids);
            crm_user_delete_normal delobj = new crm_user_delete_normal(_queueService.ExchangeName, ids);
            _queueService.PublishTopic(delobj);
        }

        public void DeleteBatch(List<Guid> ids)
        {
            DeleteCache(ids);
            crm_user_delete_normal delobj = new crm_user_delete_normal(_queueService.ExchangeName, ids);
            _queueService.PublishTopic(delobj);
        }

        private void DeleteCache(List<Guid> ids)
        {
            List<UserDto> userdtos = Mapper.Map<List<UserDto>> (_repositoryRead.GetAllList(it => ids.Contains(it.Id)));
            if (userdtos != null)
            {
                _cacheService.Cached.SortedSetRemove(CacheKeyName.CRMUserKey , userdtos);
                foreach (var userdto in userdtos)
                {
                    //if (!_cacheService.Cached.SortedSetUpdate<UserDto>(CacheKeyName.CRMUserKey, userdto, (x) => { return (x.Id == userdto.Id); }, true))
                    //    _logger.LogInformation("userDeleteCacheError: id:{0} nickname:{1}", userdto.Id, userdto.NickName);
                    DeleteCache(userdto.Id);
                }
            }
        }
        public UserDto Get(Guid id)
        {
            if (id == null) return null;
            return _cacheService.Cached.Get(() => { return Mapper.Map<UserDto>(_repositoryReadFactory.CreateRepository<User,IUserRepositoryRead>(id.ToString()).Get(id)); }, $"{CacheKeyName.CRMUserKey}{id}");
        }
        public List<UserDto> GetAllList()
        {
            return _cacheService.Cached.GetSortList<UserDto>(() =>
            {
                return Mapper.Map<List<UserDto>>(_repositoryRead.GetAllList(it => it.Id != Guid.Empty));
            }, CacheKeyName.CRMUserKey);
            // return Mapper.Map<List<UserDto>>(_repositoryRead.GetAllList(it => it.Id != Guid.Empty).OrderBy(it => it.UserName));
        }

        public List<UserDto> GetChildrenByDepartment( int startPage, int pageSize, out int rowCount)
        {
            List<UserDto> rlist = GetAllList();
            return _pagedHelper.Paged(rlist, startPage, pageSize, out rowCount
                ,(x) =>{
                        for (int i = 0; i < x.Count; i++) //此处不用foreach，要改变元素值
                        {
                                x[i] = _cacheService.Cached.Get(()=> { return Mapper.Map<UserDto>(_repositoryReadFactory.CreateRepository<User,IUserRepositoryRead>(x[i].Id.ToString()).Get(x[i].Id)); }, $"{CacheKeyName.CRMUserKey}{x[i].Id}");
                        };
                    return x;
                });
        }

        public bool Update(UserDto dto)
        {
            UserDto cuser,olddto=null;
            olddto = Get(dto.Id);
            cuser = olddto.GetCopy();
                cuser.NickName = dto.NickName;
                cuser.EMail = dto.EMail;
                cuser.MobileNumber = dto.MobileNumber;
                cuser.Address = dto.Address;

            crm_user_update_rpc user =null;
            Request(cuser, olddto, user,0);  //异步rpc的方式
            //var user = _repository.InsertOrUpdate(Mapper.Map<User>(dto));
            return true;
        }

        public bool Insert(UserDto dto)
        {
            UserDto cuser = dto;
            crm_user_update_rpc user = null;
            Request(cuser,null, user, 0);  //异步rpc的方式
            //var user = _repository.InsertOrUpdate(Mapper.Map<User>(dto));
            return true;
        }
        private void Request(UserDto dto, UserDto olddto, crm_user_update_rpc replyMsg, int runcnt)
        {
            crm_user_update_rpc msg = new crm_user_update_rpc(_queueService.ExchangeName, dto); 
            _queueService.Request<UserDto>(dto, olddto, msg, replyMsg, (x,z,y) =>
            {
                x.Id = ByteConvertHelper.Bytes2Object<Guid>(y.MessageBodyReturnByte);
                _logger.LogInformation("user.update.insertupdate.rpc: username:{0} method:{1}", x.Id, "InsertOrUpdate");
                InsertOrUpdateCache(x,z);
            }, runcnt);
        }
        private void InsertOrUpdateCache(UserDto userdto, UserDto olddto)
        {
            if (userdto != null)
            {
                if (!_cacheService.Cached.SortedSetUpdate(CacheKeyName.CRMUserKey , olddto, userdto))
                    //if (!_cacheService.Cached.SortedSetUpdate(CacheKeyName.CRMUserKey,userdto,(x)=> { return (x.Id == userdto.Id); }))
                    _logger.LogInformation("userInsertOrUpdateCacheError: id:{0} nikename:{1}", userdto.Id, userdto.NickName);
                DeleteCache(userdto.Id);
            }
        }
        
        /// <summary>
        /// 获取用户的权限列表
        /// </summary>
        /// <param name="id">userid</param>
        /// <returns></returns>
        public List<UserAddressDto> GetUserAddresss(Guid userid)
        {
            if (userid == null || default(Guid).Equals(userid)) return null;
            string tkey = $"{CacheKeyName.UserRoleKey}{userid}";
            return _cacheService.Cached.Get<List<UserAddressDto>>(() => { return Mapper.Map<List<UserAddressDto>>(_repositoryReadFactory.CreateRepository<User,IUserRepositoryRead>(userid.ToString()).GetUserAddresss(userid)); }, tkey, default(TimeSpan));
        }
        private void DeleteCache(Guid id)
        {
            List<string> keys = new List<string>(3) { $"{CacheKeyName.UserRoleKey}{id}", $"{CacheKeyName.CRMUserKey}{id}", $"{CacheKeyName.UserRoleKey}" };
            foreach (var item in keys)//RemoveAllAsync 需要key落在同一个solt上
                _cacheService.Cached.RemoveAsync(item);
        }
        public void UpdateUserAddresss(Guid id, List<int> addressIds)
        { 
            var userIds = new List<Guid>(1){ id };
            DeleteCache(id);
            _queueService.PublishTopic(new crm_useraddress_update_normal(_queueService.ExchangeName, new UserAddressMsg() { userIds = userIds, AddressIds = addressIds }));
            //_repository.UpdateUserRoles(id, Mapper.Map<List<UserRole>>(userRoles));

        }
        public void BatchUpdateUserAddresss(List<Guid> userIds, List<int> addressIds)
        {
            userIds.ForEach(x =>
            {
                DeleteCache(x);
            });
            _queueService.PublishTopic(new crm_useraddress_update_normal(_queueService.ExchangeName, new UserAddressMsg() { userIds = userIds, AddressIds = addressIds }));
            //_repository.BatchUpdateUserRoles(userIds, Mapper.Map<List<UserRole>>(userRoles));
        }

    }
}
