using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Jy.Application.DepartmentApp;
using Jy.AuthAdmin.API.Models;
using Microsoft.AspNetCore.Authorization;
using Jy.MVCAuthorization;
using Jy.Domain.Dtos;

namespace Jy.AuthAdmin.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [BearerAuthorize]
    public class DepartmentController : JyControllerBase
    {
        private readonly IDepartmentAppService _service;
        public DepartmentController(IDepartmentAppService service)
        {
            _service = service;
        }

        /// <summary>
        /// 获取功能树Ok数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [ResponseCache(Duration = 20, CacheProfileName = "departmentTree")]
        public IActionResult GetTreeData()
        {
            var dtos = _service.GetAllList();
            List<TreeModel> treeModels = new List<TreeModel>();
            foreach (var dto in dtos)
            {
                treeModels.Add(new TreeModel() { Id = dto.Id.ToString(), Text = dto.Name, Parent = dto.ParentId == Guid.Empty ? "#" : dto.ParentId.ToString() });
            }
            return Ok(treeModels);
        }

        /// <summary>
        /// 获取子级列表
        /// </summary>
        /// <param name="parentId">父id</param>
        /// <param name="startPage">开始页数</param>
        /// <param name="pageSize">每页记录</param>
        /// <returns></returns>
        // GET api/v1/[controller]/GetChildrenByParent/1[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]/{parentId}")]
        [ResponseCache(Duration = 20, VaryByQueryKeys = new string[] { "parentId", "startPage", "pageSize" })]
        public IActionResult GetChildrenByParent(Guid parentId, [FromQuery]int startPage, [FromQuery] int pageSize)
        {
            int rowCount = 0;
            var result = _service.GetChildrenByParent(parentId, startPage, pageSize, out rowCount);
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
        /// <param name="dto">DepartmentDto</param>
        /// <returns></returns>
        // POST api/Department
        [HttpPost]
        public IActionResult Create([FromBody]DepartmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            if (_service.InsertOrUpdate(dto))
            {
                return Ok(new { Result = "Success" });
            }
            return Ok(new { Result = "Faild" });
        }
        /// <summary>
        /// 编辑
        /// </summary>
        /// <param name="dto">DepartmentDto</param>
        /// <returns></returns>
        // PUT api/Department
        [HttpPut]
        public IActionResult Edit([FromBody]DepartmentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            if (_service.InsertOrUpdate(dto))
            {
                return Ok(new { Result = "Success" });
            }
            return Ok(new { Result = "Faild" });
        }
        /// <summary>
        /// 删除批量
        /// </summary>
        /// <param name="ids">string ids 1,2,3</param>
        /// <returns></returns>
        // DELETE api/Department/DeleteMuti/5,6
        [Route("[action]/{ids}")]
        [HttpDelete]
        public IActionResult DeleteMuti(string ids)
        {
            try
            {
                List<Guid> delIds = GetList(ids,',');
                _service.DeleteBatch(delIds);
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
        /// <param name="id">department id</param>
        /// <returns></returns>
        // DELETE api/Department/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                _service.Delete(id);
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
        // GET api/Department/5
        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            var dto = _service.Get(id);
            return Ok(dto);
        }
    }
}
