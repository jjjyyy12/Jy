using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Jy.Application.MenuApp;
using Jy.AuthAdmin.API.Models;
using Jy.Utility.Convert;
using Jy.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Jy.MVCAuthorization;
using Jy.Domain.Dtos;

namespace Jy.AuthAdmin.API.Controllers
{
    /// <summary>
    /// 功能管理控制器
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [BearerAuthorize]
    public class MenuController : JyControllerBase
    {
        private readonly IMenuAppService _menuAppService;
        public MenuController(IMenuAppService menuAppService)
        {
            _menuAppService = menuAppService;
        }

        /// <summary>
        /// 获取功能树数据
        /// </summary>
        /// <returns></returns>
        // GET api/v1/[controller]/GetMenuTreeData
        [HttpGet]
        [Route("[action]")]
        [ResponseCache(Duration = 20, CacheProfileName = "menuTree")]
        public IActionResult GetMenuTreeData()
        {
            var menus = _menuAppService.GetAllList();
            List<TreeModel> treeModels = new List<TreeModel>();
            foreach (var menu in menus)
            {
                treeModels.Add(new TreeModel() { Id = menu.Id.ToString(), Text = menu.Name, Parent = menu.ParentId == Guid.Empty ? "#" : menu.ParentId.ToString() });
            }
            return Ok(treeModels);
        }
        /// <summary>
        /// 获取子级功能列表
        /// </summary>
        /// <param name="parentId">父id</param>
        /// <param name="startPage">开始页数</param>
        /// <param name="pageSize">每页记录</param>
        /// <returns></returns>
        // GET api/v1/[controller]/GetMenusByParent/1[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]/{parentId}")]
        [ResponseCache(Duration = 20, VaryByQueryKeys = new string[] {"parentId", "startPage", "pageSize" })]
        public IActionResult GetMenusByParent(Guid parentId, [FromQuery]int startPage, [FromQuery] int pageSize)
        {
            int rowCount = 0;
            var result = _menuAppService.GetMenusByParent(parentId, startPage, pageSize, out rowCount);
            return Ok(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }
        /// <summary>
        /// 新增或编辑功能
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
         // POST api/Menu
        [HttpPost]
        public IActionResult Create([FromBody]MenuDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
          
            if (_menuAppService.InsertOrUpdate(dto))
            {
                return Ok(new { Result = "Success" });
            }
            return Ok(new { Result = "Faild" });
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="dto">MenuDto</param>
        /// <returns></returns>
        // PUT api/Menu
        [HttpPut]
        public IActionResult Edit([FromBody]MenuDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            MenuDto cMenu = _menuAppService.Get(dto.Id);
                cMenu.Url = dto.Url;
                cMenu.Icon = dto.Icon;
                cMenu.SerialNumber = dto.SerialNumber;
                cMenu.Name = dto.Name;
                cMenu.Code = dto.Code;
                cMenu.Remarks = dto.Remarks;

            if (_menuAppService.InsertOrUpdate(cMenu))
            {
                return Ok(new { Result = "Success" });
            }
            return Ok(new { Result = "Faild" });
        }
        /// <summary>
        /// 删除批量
        /// </summary>
        /// <param name="ids">id,id,id</param>
        /// <returns></returns>
        // DELETE api/Menu/DeleteMuti/5,6
        [Route("[action]/{ids}")]
        [HttpDelete]
        public IActionResult DeleteMuti(string ids)
        {
            try
            {
                List<Guid> delIds = GetList(ids,',');
                _menuAppService.DeleteBatch(delIds);
                return Ok(new
                {
                    Result = "Success"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Result = "Faild",
                    Message = ex.Message
                });
            }
        }
        /// <summary>
        /// 删除单个
        /// </summary>
        /// <param name="id">menu id</param>
        /// <returns></returns>
        // DELETE api/Menu/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _menuAppService.Delete(id);
                return Ok(new
                {
                    Result = "Success"
                });
            }
            catch (Exception ex)
            {
                return Ok(new
                {
                    Result = "Faild",
                    Message = ex.Message
                });
            }
        }
        // GET api/Menu/5
        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            var dto = _menuAppService.Get(id);
            return Ok(dto);
        }
    }
}
