using System;
using System.Collections.Generic;
using System.Linq;
using Jy.Domain.Dtos;
using Jy.Domain.IRepositories;
using AutoMapper;
using Jy.Utility.Convert;
using Jy.ILog;
using Jy.Utility.Paged;
using Jy.Utility.Const;
using Jy.CacheService;
using Jy.QueueSerivce;
using Jy.Domain.Message;

namespace Jy.Application.DepartmentApp
{
    public class DepartmentAppService : IDepartmentAppService
    {
        private readonly IDepartmentRepositoryRead _repositoryRead;
        private readonly ICacheService _cacheService;
        //缓存 
        public ICacheService CacheService { get { return _cacheService; } }

        private readonly IQueueService _queueService;
        //队列
        public IQueueService QueueService { get { return _queueService; } }

        private ILogger _logger;
        private PagedHelper _pagedHelper;
        public DepartmentAppService(IDepartmentRepositoryRead repository, ICacheService cacheService, IQueueService queueService, ILogger logger, PagedHelper pagedHelper)
        {
            _repositoryRead = repository;
            _cacheService = cacheService;
            _queueService = queueService;
            _logger = logger;
            _pagedHelper = pagedHelper;
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public List<DepartmentDto> GetAllList()
        {
            return _cacheService.Cached.GetSortList<DepartmentDto>(() => {
                return Mapper.Map<List<DepartmentDto>>(_repositoryRead.GetAllList(it => it.Id != Guid.Empty).OrderBy(it => it.Name));
            }, CacheKeyName.DepartmentKey);
            //return _cacheService.Cached.Get<List<DepartmentDto>>(() => { return Mapper.Map<List<DepartmentDto>>(_repository.GetAllList(it => it.Id != Guid.Empty).OrderBy(it => it.Code)); }, CacheKeyName.DepartmentKey, default(TimeSpan));
        }

        /// <summary>
        /// 根据父级Id获取子级列表
        /// </summary>
        /// <param name="parentId">父级Id</param>
        /// <param name="startPage">起始页</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="rowCount">数据总数</param>
        /// <returns></returns>
        public List<DepartmentDto> GetChildrenByParent(Guid parentId, int startPage, int pageSize, out int rowCount)
        {
            List<DepartmentDto> rlist = GetAllList().Where(x => x.ParentId == parentId).ToList();
            rowCount = rlist.Count();
            return rlist.Skip((startPage - 1) * pageSize).Take(pageSize).ToList();
            //return Mapper.Map<List<DepartmentDto>>(_repository.LoadPageList(startPage, pageSize, out rowCount, it => it.ParentId == parentId, it => it.Code));
        }

        /// <summary>
        /// 新增或修改
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        public bool InsertOrUpdate(DepartmentDto dto)
        {
            //var department = _repository.InsertOrUpdate(Mapper.Map<Department>(dto));
            department_update_insertupdate_rpc msg = null;
            Request(dto, msg,0);  //异步rpc的方式
            return true;
        }
        private void Request(DepartmentDto dto, department_update_insertupdate_rpc replyMsg, int runcnt)
        {
            department_update_insertupdate_rpc msg = new department_update_insertupdate_rpc(_queueService.ExchangeName, dto);
            _queueService.Request<DepartmentDto>(dto, msg, replyMsg, (x, y) =>
            {
                x.Id = ByteConvertHelper.Bytes2Object<Guid>(y.MessageBodyReturnByte);
                _logger.LogInformation("department.update.insertupdate.rpc: name:{0} method:{1}", x.Id, "InsertOrUpdate");
                InsertOrUpdateCache(x);
            }, runcnt);
        }

        private void InsertOrUpdateCache(DepartmentDto inobj)
        {
            if (inobj != null)
            {
                if (!_cacheService.Cached.SortedSetUpdate<DepartmentDto>(CacheKeyName.DepartmentKey, inobj, (x) => { return (x.Id == inobj.Id); }))
                    _logger.LogInformation("departmentInsertOrUpdateCacheError: username:{0} method:{1}", inobj.Id, inobj.Name);
                DeleteCache(inobj.Id);
            }
        }
        /// <summary>
        /// 根据Id集合批量删除
        /// </summary>
        /// <param name="ids">Id集合</param>
        public void DeleteBatch(List<Guid> ids)
        {
            DeleteCache(ids);
            _queueService.PublishTopic(new department_delete_deletedepartment_normal(_queueService.ExchangeName, ids));
            //_repository.Delete(it => ids.Contains(it.Id));
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">Id</param>
        public void Delete(Guid id)
        {
            if (id == null || default(Guid).Equals(id)) return;
            List<Guid> ids = new List<Guid>(1); ids.Add(id);
            DeleteCache(ids);
            _queueService.PublishTopic(new department_delete_deletedepartment_normal(_queueService.ExchangeName, ids));
            //_repository.Delete(id);
        }
        private void DeleteCache(List<Guid> ids)
        {
            List<DepartmentDto> userdtos = Mapper.Map<List<DepartmentDto>>(_repositoryRead.GetAllList(it => ids.Contains(it.Id)));
            if (userdtos != null)
            {
                foreach (var userdto in userdtos)
                {
                    if (!_cacheService.Cached.SortedSetUpdate<DepartmentDto>(CacheKeyName.DepartmentKey, userdto, (x) => { return (x.Id == userdto.Id); }, true))
                        _logger.LogInformation("departmentDeleteCacheError: username:{0} method:{1}", userdto.Id, userdto.Name);
                    DeleteCache(userdto.Id);
                }
            }
        }
        private void DeleteCache(Guid id)
        {
            if (id == null || default(Guid).Equals(id)) return;
            List<string> keys = new List<string>(1) { $"{CacheKeyName.DepartmentKey}{id}" };
            foreach (var item in keys)//RemoveAllAsync 需要key落在同一个solt上
                _cacheService.Cached.RemoveAsync(item);
        }
        /// <summary>
        /// 根据Id获取实体
        /// </summary>
        /// <param name="id">Id</param>
        /// <returns></returns>
        public DepartmentDto Get(Guid id)
        {
            if (id == null || default(Guid).Equals(id)) return null;
            return _cacheService.Cached.Get(() => { return Mapper.Map<DepartmentDto>(_repositoryRead.Get(id)); }, $"{CacheKeyName.DepartmentKey}{id}");
            //return Mapper.Map<DepartmentDto>(_repositoryRead.Get(id));
        }
    }
}
