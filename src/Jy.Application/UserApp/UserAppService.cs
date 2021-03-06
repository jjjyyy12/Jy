﻿using System;
using System.Collections.Generic;
using System.Linq;
using Jy.Domain.Dtos;
using Jy.Domain.Entities;
using Jy.Domain.IRepositories;
using AutoMapper;
using Jy.Utility.Convert;
using Jy.Utility.Paged;
using Jy.Domain.Message;
using Jy.ILog;
using Jy.Utility.Const;
using Jy.CacheService;
using Jy.QueueSerivce;
using Jy.IRepositories;

namespace Jy.Application.UserApp
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
            if (id == null || default(Guid).Equals(id)) return;
            List<Guid> ids = new List<Guid>(1); ids.Add(id);
            DeleteCache(ids);
            user_delete_deleteuser_normal delobj = new user_delete_deleteuser_normal(_queueService.ExchangeName, ids);
            _queueService.PublishTopic(delobj);
        }

        public void DeleteBatch(List<Guid> ids)
        {
            DeleteCache(ids);
            user_delete_deleteuser_normal delobj = new user_delete_deleteuser_normal(_queueService.ExchangeName, ids);
            _queueService.PublishTopic(delobj);
        }

        private void DeleteCache(List<Guid> ids)
        {
            List<UserDto> userdtos = Mapper.Map<List<UserDto>> (_repositoryRead.GetAllList(it => ids.Contains(it.Id)));
            if (userdtos != null && userdtos.Count>0)
            {
                var departmentid = userdtos[0].DepartmentId;
                _cacheService.Cached.SortedSetRemove($"{CacheKeyName.UserKey}{departmentid}", userdtos);
                foreach (var userdto in userdtos)
                {
                    //if (!_cacheService.Cached.SortedSetUpdate<UserDto>(CacheKeyName.UserKey + userdto.DepartmentId, userdto, (x) => { return (x.Id == userdto.Id); }, true))
                    //    _logger.LogInformation("userDeleteCacheError: username:{0} method:{1}", userdto.Id, userdto.Name);
                    DeleteCache(userdto.Id);
                }
            }
        }
        public UserDto Get(Guid id)
        {
            if (id == null) return null;
            return _cacheService.Cached.Get(() => { return Mapper.Map<UserDto>(_repositoryReadFactory.CreateRepository<User,IUserRepositoryRead>(id.ToString()).Get(id)); }, $"{CacheKeyName.UserKey}{id}");
            //return rlist.FirstOrDefault(x => x.Id == id);
           // return Mapper.Map<UserDto>(_repositoryReadFactory.CreateUserRepositoryRead(id.ToString()).Get(id));
        }
        //[Obsolete]
        //public List<UserDto> GetAllList()
        //{
        //    return _cacheService.Cached.GetSortList<UserDto>(() => {
        //        return Mapper.Map<List<UserDto>>(_repositoryRead.GetAllList(it => it.Id != Guid.Empty).OrderBy(it => it.UserName));
        //    }, CacheKeyName.UserKey);
        //   // return Mapper.Map<List<UserDto>>(_repositoryRead.GetAllList(it => it.Id != Guid.Empty).OrderBy(it => it.UserName));
        //}

        public List<UserDto> GetChildrenByDepartment(Guid DepartmentId, int startPage, int pageSize, out int rowCount)
        {
            var res = new List<UserDto>();
            var key = $"{CacheKeyName.UserKey}{DepartmentId}";
            if (_cacheService.Cached.Exists(key))
            {
                rowCount = (int)_cacheService.Cached.SortedSetLength(key);
                res = _cacheService.Cached.SortedSetRangeByRank<UserDto>(key, startPage == 1 ? 0 : (startPage - 1) * pageSize, (startPage) * pageSize - 1);
            }
            else
            {
                List<UserDto> rlist = _cacheService.Cached.GetSortList<UserDto>(() => {
                    return Mapper.Map<List<UserDto>>(_repositoryRead.GetUserIndexList(it => it.DepartmentId == DepartmentId).OrderBy(it => it.UserName));
                }, key);
                res = _pagedHelper.Paged(rlist, startPage, pageSize, out rowCount);
            }
            for (int i = 0; i < res.Count; i++) //此处不用foreach，要改变元素值
            {
                res[i] = _cacheService.Cached.Get(() => { return Mapper.Map<UserDto>(_repositoryReadFactory.CreateRepository<User, IUserRepositoryRead>(res[i].Id.ToString()).Get(res[i].Id)); }, $"{CacheKeyName.UserKey}{res[i].Id}");
                if (res[i].CreateUserId != null)
                {
                    User tuser = _cacheService.Cached.Get(() => { return _repositoryReadFactory.CreateRepository<User, IUserRepositoryRead>(res[i].CreateUserId.ToString()).Get(res[i].CreateUserId); }, $"{CacheKeyName.UserKey}{res[i].CreateUserId}");
                    if (tuser != null)
                    {
                        res[i].CreateUserName = tuser.UserName;
                    }
                }
            }
            return res;
            //IQueryable<User> tu = _repositoryRead.LoadPageList(startPage, pageSize, out rowCount, it => it.DepartmentId == DepartmentId, it => it.UserName);
            //List<User> il = tu.ToList();
            //il.ForEach(it => it.CreateUser = _repositoryRead.Get(it.CreateUserId));
            //return Mapper.Map<List<UserDto>>(il);
        }

        public bool InsertOrUpdate(UserDto dto,UserDto createUser)
        {
            UserDto cuser,olddto = null;
            if (default(Guid).Equals(dto.Id)) //add
            {
                cuser = dto;
                if (createUser != null)
                {
                    cuser.CreateUserId = createUser.Id;
                    cuser.CreateTime = DateTime.Now;
                }
            }
            else //edit
            {
                olddto = Get(dto.Id);
                cuser = olddto.GetCopy();
                cuser.Name = dto.Name;
                cuser.UserName = dto.UserName;
                cuser.Email = dto.Email;
                cuser.MobileNumber = dto.MobileNumber;
                cuser.Remarks = dto.Remarks;
            }

            user_update_insertupdate_rpc user =null;
            Request(cuser, olddto, user,0);  //异步rpc的方式
            //var user = _repository.InsertOrUpdate(Mapper.Map<User>(dto));
            return true;
        }
        private void Request(UserDto dto, UserDto olddto, user_update_insertupdate_rpc replyMsg, int runcnt)
        {
            user_update_insertupdate_rpc msg = new user_update_insertupdate_rpc(_queueService.ExchangeName, dto); 
            _queueService.Request<UserDto>(dto, olddto, msg, replyMsg, (x,z,y) =>
            {
                x.Id = ByteConvertHelper.Bytes2Object<Guid>(y.MessageBodyReturnByte);
                _logger.LogInformation("user.update.insertupdate.rpc: username:{0} method:{1}", x.Id, "InsertOrUpdate");
                InsertOrUpdateCache(x,z);
            }, runcnt);
        }
        private void InsertOrUpdateCache(UserDto userdto,UserDto olddto)
        {
            if (userdto != null)
            {
                if(!_cacheService.Cached.SortedSetUpdate($"{CacheKeyName.UserKey}{userdto.DepartmentId}", olddto, userdto))
                //if(!_cacheService.Cached.SortedSetUpdate(CacheKeyName.UserKey + userdto.DepartmentId,userdto,(x)=> { return (x.Id == userdto.Id); }))
                    _logger.LogInformation("userInsertOrUpdateCacheError: username:{0} method:{1}", userdto.Id, userdto.Name);
                DeleteCache(userdto.Id);
            }
        }
        
        /// <summary>
        /// 获取用户的权限列表
        /// </summary>
        /// <param name="id">userid</param>
        /// <returns></returns>
        public List<UserRoleDto> GetUserRoles(Guid userid)
        {
            if (userid == null || default(Guid).Equals(userid)) return null;
            string tkey = $"{CacheKeyName.UserRoleKey}{userid}";
            return _cacheService.Cached.Get<List<UserRoleDto>>(() => { return Mapper.Map<List<UserRoleDto>>(_repositoryReadFactory.CreateRepository<User,IUserRepositoryRead>(userid.ToString()).GetUserRoles(userid)); }, tkey, default(TimeSpan));
        }
        private void DeleteCache(Guid id)
        {
            if (id == null || default(Guid).Equals(id)) return;
            List<string> keys = new List<string>(2) { $"{CacheKeyName.UserRoleKey}{id}", $"{CacheKeyName.UserKey}{id}" };
            foreach (var item in keys)//RemoveAllAsync 需要key落在同一个solt上
                _cacheService.Cached.RemoveAsync(item);
        }
        public void UpdateUserRoles(Guid id, List<Guid> roleIds)
        { 
            var userIds = new List<Guid>(1){ id };
            DeleteCache(id);
            _queueService.PublishTopic(new user_update_userroles_normal(_queueService.ExchangeName, new UserRoleMsg() { userIds = userIds, roleIds = roleIds }));
            //_repository.UpdateUserRoles(id, Mapper.Map<List<UserRole>>(userRoles));

        }
        public void BatchUpdateUserRoles(List<Guid> userIds, List<Guid> roleIds)
        {
            userIds.ForEach(x =>
            {
                DeleteCache(x);
            });
            _queueService.PublishTopic(new user_update_userroles_normal(_queueService.ExchangeName, new UserRoleMsg() { userIds = userIds, roleIds = roleIds }));
            //_repository.BatchUpdateUserRoles(userIds, Mapper.Map<List<UserRole>>(userRoles));
        }

    }
}
