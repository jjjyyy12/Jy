﻿using System;
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

namespace Jy.Application.MenuApp
{
    public class MenuAppService : IMenuAppService
    {
        private readonly IMenuRepositoryRead _menuRepositoryRead;
        private readonly ICacheService _cacheService;
        //缓存 
        public ICacheService CacheService { get { return _cacheService; } }

        private readonly IQueueService _queueService;
        //队列
        public IQueueService QueueService { get { return _queueService; } }

        private ILogger _logger;
        private PagedHelper _pagedHelper;
        public MenuAppService(IMenuRepositoryRead menuRepository, ICacheService cacheService, IQueueService queueService, ILogger logger, PagedHelper pagedHelper)
        {
            _menuRepositoryRead = menuRepository;
            _cacheService = cacheService;
            _queueService = queueService;
            _logger = logger;
            _pagedHelper = pagedHelper;
        }

        public List<MenuDto> GetAllList()
        {
            return _cacheService.Cached.GetSortList<MenuDto>(() => {
                return Mapper.Map<List<MenuDto>>(_menuRepositoryRead.GetAllList(it => it.Id != Guid.Empty).OrderBy(it => it.SerialNumber));
            }, CacheKeyName.MenuKey);
            //return _cacheService.Cached.Get<List<MenuDto>>(() => {return Mapper.Map<List<MenuDto>>(_menuRepository.GetAllList().OrderBy(it => it.SerialNumber));}, CacheKeyName.MenuKey, default(TimeSpan));
        }

        public List<MenuDto> GetMenusByParent(Guid parentId, int startPage, int pageSize, out int rowCount)
        {
            List<MenuDto> rlist = GetAllList().Where(x => x.ParentId == parentId).ToList();
            return _pagedHelper.Paged<MenuDto>(rlist, startPage, pageSize, out rowCount);
            //var menus = _menuRepository.LoadPageList(startPage, pageSize, out rowCount, it => it.ParentId == parentId, it => it.SerialNumber);
            //return Mapper.Map<List<MenuDto>>(menus);
        }

        public bool InsertOrUpdate(MenuDto dto)
        {
            MenuDto cMenu, olddto = null;
            if (default(Guid).Equals(dto.Id)) //add
            {
                cMenu = dto;
            }
            else //edit
            {
                olddto = Get(dto.Id);
                cMenu = olddto.GetCopy();
                cMenu.Url = dto.Url;
                cMenu.Icon = dto.Icon;
                cMenu.SerialNumber = dto.SerialNumber;
                cMenu.Name = dto.Name;
                cMenu.Code = dto.Code;
                cMenu.Remarks = dto.Remarks;
            }

            //var menu = _menuRepository.InsertOrUpdate(Mapper.Map<Menu>(dto));
            menu_update_insertupdate_rpc msg = null;
            Request(dto, olddto, msg,0);  //异步rpc的方式
            return true;
        }
        private void Request(MenuDto dto, MenuDto olddto, menu_update_insertupdate_rpc replyMsg, int runcnt)
        {
            menu_update_insertupdate_rpc msg = new menu_update_insertupdate_rpc(_queueService.ExchangeName, dto);
             _queueService.Request(dto, olddto, msg, replyMsg, (x,z, y) =>
            {
                x.Id = ByteConvertHelper.Bytes2Object<Guid>(y.MessageBodyReturnByte);
                _logger.LogInformation("menu.update.insertupdate.rpc: name:{0} method:{1}", x.Id, "InsertOrUpdate");
                InsertOrUpdateCache(x,z);
            }, runcnt);
        }

        private void InsertOrUpdateCache(MenuDto inobj,MenuDto olddto)
        {
            if (inobj != null)
            {
                if (!_cacheService.Cached.SortedSetUpdate(CacheKeyName.MenuKey, olddto, inobj))
                    //if (!_cacheService.Cached.SortedSetUpdate<MenuDto>(CacheKeyName.MenuKey, inobj, (x) => { return (x.Id == inobj.Id); }))
                    _logger.LogInformation("menuInsertOrUpdateCacheError: username:{0} method:{1}", inobj.Id, inobj.Name);
                DeleteCache(inobj.Id);
            }
        }
        public void DeleteBatch(List<Guid> ids)
        {
            DeleteCache(ids);
            _queueService.PublishTopic(new menu_delete_deletemenu_normal(_queueService.ExchangeName, ids));
            //_menuRepository.Delete(it => ids.Contains(it.Id));
        }

        public void Delete(Guid id) 
        {
            if (id == null || default(Guid).Equals(id)) return;
            List<Guid> ids = new List<Guid>(1); ids.Add(id);
            DeleteCache(ids);
            _queueService.PublishTopic(new menu_delete_deletemenu_normal(_queueService.ExchangeName, ids));
            //_menuRepository.Delete(id);
        }
        private void DeleteCache(List<Guid> ids)
        {
            List<MenuDto> dtos = Mapper.Map<List<MenuDto>>(_menuRepositoryRead.GetAllList(it => ids.Contains(it.Id)));
            if (dtos != null)
            {
                _cacheService.Cached.SortedSetRemove(CacheKeyName.MenuKey, dtos);
                foreach (var dto in dtos)
                {
                    //if (!_cacheService.Cached.SortedSetUpdate<MenuDto>(CacheKeyName.MenuKey, dto, (x) => { return (x.Id == dto.Id); }, true))
                    //    _logger.LogInformation("menuDeleteCacheError: username:{0} method:{1}", dto.Id, dto.Name);
                    DeleteCache(dto.Id);
                }
            }
        }
        private void DeleteCache(Guid id)
        {
            if (id == null || default(Guid).Equals(id)) return;
            List<string> keys = new List<string>(1) { $"{CacheKeyName.MenuKey}{id}" };
            foreach (var item in keys)//RemoveAllAsync 需要key落在同一个solt上
                _cacheService.Cached.RemoveAsync(item);
        }
        public MenuDto Get(Guid id)
        {
            if (id == null) return null;
            return _cacheService.Cached.Get(() => { return Mapper.Map<MenuDto>(_menuRepositoryRead.Get(id)); }, $"{CacheKeyName.MenuKey}{id}");
            //return Mapper.Map<MenuDto>(_menuRepositoryRead.Get(id));
        }
    }
}
