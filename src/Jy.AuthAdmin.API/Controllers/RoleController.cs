using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Jy.Application.RoleApp;
using Jy.AuthAdmin.API.Models;
using Jy.Utility.Convert;
using Jy.Application.MenuApp;
using Jy.MVCAuthorization;
using Jy.TokenService;
using Jy.Domain.Dtos;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Jy.AuthAdmin.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [BearerAuthorize]
    public class RoleController : JyControllerBase
    {
        private readonly IRoleAppService _service;
        private readonly IMenuAppService _menuAppService;
        private readonly IVerifyTokenAppService _verifyTokenAppService;
        private IHttpContextAccessor _httpContextAccesor;
        public RoleController(IRoleAppService service , IMenuAppService menuAppService, IVerifyTokenAppService verifyTokenAppService,IHttpContextAccessor httpContextAccesor)
        {
            _service = service;
            _menuAppService = menuAppService;
            _verifyTokenAppService = verifyTokenAppService;
            _httpContextAccesor = httpContextAccesor;
        }


        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        // GET api/v1/[controller]/GetListPaged[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetListPaged([FromQuery]int startPage, [FromQuery]int pageSize)
        {
            int rowCount = 0;
            var result = _service.GetListPaged( startPage, pageSize, out rowCount);
            return Ok(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }
        // GET api/v1/[controller]/GetMenuTreeData/id
        [HttpGet]
        [Route("[action]/{id}")]
        public IActionResult GetMenuTreeData(Guid id)
        {
            var menus = _menuAppService.GetAllList();
            var roleMenus = _service.GetRowMenus(id);
            List<TreeCheckBoxModel> treeModels = new List<TreeCheckBoxModel>();

            menus.ForEach((menu) =>
            {
                RoleMenuDto tempt = roleMenus.Find((roleMenu) => menu.Id == roleMenu.MenuId);
                treeModels.Add(new TreeCheckBoxModel() { Id = menu.Id.ToString(), Text = menu.Name, Parent = menu.ParentId == Guid.Empty ? "#" : menu.ParentId.ToString(), Checked = tempt!=null?"1":"0" });
            });

            return Ok(treeModels);
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        // POST api/Role
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]RoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            UserDto currUser = null;
            var token = await GetUserTokenAsync();
            var userStatus = _verifyTokenAppService.GetCurrentUserStatus(token);
            if (userStatus != null)
                currUser = new UserDto() { Id = userStatus.UserId };

            if (_service.InsertOrUpdate(dto, currUser))
            {
                return Ok(new { Result = "Success" });
            }
            return Ok(new { Result = "Faild" });
        }
        // PUT api/Role
        [HttpPut]
        public IActionResult Edit([FromBody]RoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
           
            if (_service.InsertOrUpdate(dto, null))
            {
                return Ok(new { Result = "Success" });
            }
            return Ok(new { Result = "Faild" });
        }
        // DELETE api/Role/DeleteMuti/5,6
        [Route("[action]/{ids}")]
        [HttpDelete]
        public IActionResult DeleteMuti(string ids)
        {
            try
            {
                string[] idArray = ids.Split(',');
                List<Guid> delIds = new List<Guid>();
                foreach (string id in idArray)
                {
                    delIds.Add(Guid.Parse(id));
                }
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
        // DELETE api/Role/5
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
        [HttpPut]
        [Route("[action]")]
        public IActionResult RoleMenu([FromBody]RoleMenuModel rpm)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            try
            {
                string[] menuIDs = rpm.menuIDs?.Split('_') ;
                List<Guid> dto = new List<Guid>();
                int? j = menuIDs?.Length;
                for (int i = 0; i < j; i++)
                {
                    dto.Add(Guid.Parse(menuIDs?[i]));
                }

                _service.UpdateRowMenus(rpm.roleMenuId, dto);
          
                return Ok(new { Result = "Success" });
            }
            catch(Exception ex)
            {
                return Ok(new { Result = "Faild" , Message = ex .Message});
            }
        }
        // GET api/Role/5
        [HttpGet("{id}")]
        public ActionResult Get(Guid id)
        {
            var dto = _service.Get(id);
            return Ok(dto);
        }
        async Task<string> GetUserTokenAsync()
        {
            //var context = _httpContextAccesor.HttpContext;
            //return await context.Authentication.GetTokenAsync("access_token");
            return await _httpContextAccesor.HttpContext.GetTokenAsync("access_token");
        }
    }
}
