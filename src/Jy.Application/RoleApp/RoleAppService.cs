using System;
using System.Collections.Generic;
using System.Linq;
using Jy.Domain.Dtos;
using Jy.Domain.Entities;
using Jy.Domain.IRepositories;
using AutoMapper;
using Jy.Utility.Convert;
using Jy.Domain.Message;
using Jy.ILog;
using Jy.Utility.Paged;
using Jy.Utility.Const;
using Jy.CacheService;
using Jy.QueueSerivce;

namespace Jy.Application.RoleApp
{
    /// <summary>
    /// 角色管理服务
    /// </summary>
    public class RoleAppService : IRoleAppService
    {
        //角色管理仓储接口
        private readonly IRoleRepositoryRead _repositoryRead;
        private readonly IUserRepository _userrepository;
        private readonly ICacheService _cacheService;
        //缓存 
        public ICacheService CacheService { get { return _cacheService; } }

        private readonly IQueueService _queueService;
        //队列
        public IQueueService QueueService { get { return _queueService; } }

        private ILogger _logger;
        private PagedHelper _pagedHelper;
        /// <summary>
        /// 构造函数 实现依赖注入
        /// </summary>
        /// <param name="RoleRepository">仓储对象</param>
        public RoleAppService(IRoleRepositoryRead RoleRepository,IUserRepository UserRepository, ICacheService cacheService, IQueueService queueService, ILogger logger, PagedHelper pagedHelper)
        {
            _repositoryRead = RoleRepository;
            _userrepository = UserRepository;
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
            _queueService.PublishTopic(new role_delete_deleterole_normal(_queueService.ExchangeName, ids));
            //_repository.Delete(id);
        }

        public void DeleteBatch(List<Guid> ids)
        {
            DeleteCache(ids);
            _queueService.PublishTopic(new role_delete_deleterole_normal(_queueService.ExchangeName, ids));
            //_repository.Delete(it => ids.Contains(it.Id));
        }
        private void DeleteCache(List<Guid> ids)
        {
            List<RoleDto> dtos = Mapper.Map<List<RoleDto>>(_repositoryRead.GetAllList(it => ids.Contains(it.Id)));
            if (dtos != null)
            {
                _cacheService.Cached.SortedSetRemove(CacheKeyName.RoleKey, dtos);
                foreach (var dto in dtos)
                {
                    //if (!_cacheService.Cached.SortedSetUpdate<RoleDto>(CacheKeyName.RoleKey , dto, (x) => { return (x.Id == dto.Id); }, true))
                    //    _logger.LogInformation("userDeleteCacheError: username:{0} method:{1}", dto.Id, userdto.Name);
                    DeleteCache(dto.Id);
                }
            }
        }
        public RoleDto Get(Guid id)
        {
            if (id == null || default(Guid).Equals(id)) return null;
            return _cacheService.Cached.Get(() => { return Mapper.Map<RoleDto>(_repositoryRead.Get(id)); }, $"{CacheKeyName.RoleKey}{id}");
            //return Mapper.Map<RoleDto>(_repositoryRead.Get(id));
        }

        public List<RoleDto> GetAllList()
        {
            return _cacheService.Cached.GetSortList<RoleDto>(() => {
                return Mapper.Map<List<RoleDto>>(_repositoryRead.GetAllList(it => it.Id != Guid.Empty).OrderBy(it => it.Name));
            }, CacheKeyName.RoleKey);
            //return _cacheService.Cached.Get<List<RoleDto>>(() => { return Mapper.Map<List<RoleDto>>(_repository.GetAllList(it => it.Id != Guid.Empty).OrderBy(it => it.Code)); }, CacheKeyName.RoleKey, default(TimeSpan));
        }

        public List<RoleDto> GetListPaged( int startPage, int pageSize, out int rowCount)
        {
            //IQueryable<Role> tu = _repository.LoadPageList(startPage, pageSize, out rowCount, null, it => it.Code);
            //List<Role> il = tu.ToList();
            //il.ForEach(it => it.CreateUser = _userrepository.Get(it.CreateUserId));
            //return Mapper.Map<List<RoleDto>>(il);
            var res = new List<RoleDto>();
            var key = CacheKeyName.RoleKey;
            if (_cacheService.Cached.Exists(key))
            {
                rowCount = (int)_cacheService.Cached.SortedSetLength(key);
                res = _cacheService.Cached.SortedSetRangeByRank<RoleDto>(key, startPage == 1 ? 0 : (startPage - 1) * pageSize, (startPage) * pageSize - 1);
            }
            else
            {
                List<RoleDto> rlist = GetAllList();
                res = _pagedHelper.Paged<RoleDto>(rlist, startPage, pageSize, out rowCount);
            }

            for (int i = 0; i < res.Count; i++) //此处不用foreach，要改变元素值
            {
                User tuser = _userrepository.Get(res[i].CreateUserId);
                if (tuser != null)
                {
                    res[i].CreateUserId = tuser.Id;
                    res[i].CreateUserName = tuser.UserName;
                }
            }
            return res;
        }
        public bool InsertOrUpdate(RoleDto dto, UserDto moduser)
        {
            RoleDto cRole,olddto = null;
            if (default(Guid).Equals(dto.Id)) //add
            {
                cRole = dto;
                if (moduser != null)
                {
                    cRole.CreateUserId = moduser.Id;
                    cRole.CreateTime = DateTime.Now;
                }
            }
            else //edit
            {
                olddto = Get(dto.Id);
                cRole = olddto.GetCopy();
                cRole.Name = dto.Name;
                cRole.Code = dto.Code;
                cRole.Remarks = dto.Remarks;
            }
            role_update_insertupdate_rpc msg = null;
            Request(cRole,olddto, msg,0);  //异步rpc的方式
            //var user = _repository.InsertOrUpdate(Mapper.Map<Role>(dto));
            return true;
        }
        private void Request(RoleDto dto, RoleDto olddto, role_update_insertupdate_rpc replyMsg, int runcnt)
        {
            role_update_insertupdate_rpc msg = new role_update_insertupdate_rpc(_queueService.ExchangeName,dto);
            _queueService.Request<RoleDto>(dto, olddto,msg, replyMsg, (x,z, y) =>
            {
                x.Id = ByteConvertHelper.Bytes2Object<Guid>(y.MessageBodyReturnByte);
                _logger.LogInformation("role.update.insertupdate.rpc: name:{0} method:{1}", x.Id, "InsertOrUpdate");
                InsertOrUpdateCache(x,z);
            }, runcnt);
        }

        private void InsertOrUpdateCache(RoleDto inobj,RoleDto olddto)
        {
            if (inobj != null)
            {
                if (!_cacheService.Cached.SortedSetUpdate(CacheKeyName.RoleKey, olddto, inobj))
                    //if (!_cacheService.Cached.SortedSetUpdate<RoleDto>(CacheKeyName.RoleKey, inobj, (x) => { return (x.Id == inobj.Id); }))
                   _logger.LogInformation("roleInsertOrUpdateCacheError: username:{0} method:{1}", inobj.Id, inobj.Name);
                DeleteCache(inobj.Id);
            }
        }
        public List<RoleMenuDto> GetRoleMenus(Guid id)
        {
            string tkey = $"{CacheKeyName.RoleMenuKey}{id}";
            return _cacheService.Cached.Get<List<RoleMenuDto>>(() => { return Mapper.Map<List<RoleMenuDto>>(_repositoryRead.GetRoleMenus(id)); }, tkey, default(TimeSpan));
            //return Mapper.Map<List<RoleMenuDto>>(_repository.GetRoleMenus(id));
        }

        public void UpdateRoleMenus(Guid id, List<Guid> menuIds)
        {
            DeleteCache(id);
            _queueService.PublishTopic(new role_update_rolemenus_normal(_queueService.ExchangeName, new RoleMenuMsg() { Id = id, menuIds = menuIds }));
            //_repository.UpdateRoleMenus(id, Mapper.Map<List<RoleMenu>>(roleMenus));
        }
        private void DeleteCache(Guid id)
        {
            if (id == null || default(Guid).Equals(id)) return;
            List<string> keys = new List<string>(3) { $"{CacheKeyName.RoleMenuKey}{id}", $"{CacheKeyName.RoleMenuKey}",$"{CacheKeyName.RoleKey}{id}" };
            foreach(var item in keys)//RemoveAllAsync 需要key落在同一个solt上
                _cacheService.Cached.RemoveAsync(item);
        }
        public List<RoleMenuDto> GetAllRoleMenus()
        {
            string tkey = CacheKeyName.RoleMenuKey;
            return _cacheService.Cached.Get<List<RoleMenuDto>>(() => { return Mapper.Map<List<RoleMenuDto>>(_repositoryRead.GetAllRoleMenus()); }, tkey, default(TimeSpan));
        }
        
        public List<string> GetUserRoleMenusUrls(List<Guid> roleIds)
        {
            List<RoleMenuDto> rlist = GetUserRoleMenus(roleIds);
            List<string> slist = Mapper.Map<List<string>>(rlist);
            slist.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            return slist;
        }
        public List<RoleMenuDto> GetRoleMenuForLeftMenu(List<Guid> roleIds)
        {
            List<RoleMenuDto> rlist = GetUserRoleMenus(roleIds);
            rlist.RemoveAll(x => string.IsNullOrWhiteSpace(x.MenuName));
            return rlist.Distinct(new RoleMenuDtoComparer()).ToList();
        }
        private List<RoleMenuDto> GetUserRoleMenus(List<Guid> roleIds)
        {
            List<RoleMenuDto> rlist = GetAllRoleMenus().Where(t => roleIds.Contains(t.RoleId)).ToList();
            return rlist;
        }
    }
}
