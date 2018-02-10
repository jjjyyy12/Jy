using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Jy.MVC.Models;
using Jy.Utility.Convert;
using Jy.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Jy.MVCAuthorization;
using Jy.Domain.Dtos;
using Jy.MVC.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Jy.MVC.Controllers
{
    /// <summary>
    /// 功能管理控制器
    /// </summary>
    [BearerAuthorize]
    public class MenuController : JyControllerBase
    {
        private readonly IMenuService _service;
        public MenuController(IMenuService service)
        {
            _service = service;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取功能树JSON数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMenuTreeData()
        {
            var res = await _service.GetMenuTreeData();
            return Json(res);
        }
        /// <summary>
        /// 获取子级功能列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMenusByParent(Guid parentId, int startPage, int pageSize)
        {
            var res = await _service.GetMenusByParent(parentId,startPage,pageSize);
            return Json(res);
        }
        /// <summary>
        /// 新增或编辑功能
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Models.Menu dto)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            if (default(Guid).Equals(dto.Id)) //add
            {
                var res = await _service.Create(dto);
                return Json(res);
            }
            else
            {
                var res = await _service.Edit(dto);
                return Json(res);
            } //edit
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMuti(string ids)
        {
            var res = await _service.DeleteMuti(ids);
            return Json(res);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var res = await _service.Delete(id);
            return Json(res);
        }
        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var res = await _service.Get(id);
            return Json(res);
        }
    }
}
