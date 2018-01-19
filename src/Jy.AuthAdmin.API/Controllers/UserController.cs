using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Jy.Application.UserApp;
using Jy.AuthAdmin.API.Models;
using Jy.Application.DepartmentApp;
using Jy.MVCAuthorization;
using Jy.TokenService;
using Jy.Domain.Dtos;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Jy.AuthAdmin.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [BearerAuthorize]
    public class UserController : JyControllerBase
    {
        private readonly IUserAppService _service;
        private readonly IDepartmentAppService _departmentservice;
        private readonly IVerifyTokenAppService _verifyTokenAppService;
        private IHttpContextAccessor _httpContextAccesor;
        public UserController(IUserAppService service, IDepartmentAppService departmentservice, IVerifyTokenAppService verifyTokenAppService, IHttpContextAccessor httpContextAccesor)
        {
            _service = service;
            _departmentservice = departmentservice;
            _verifyTokenAppService = verifyTokenAppService;
            _httpContextAccesor = httpContextAccesor;
        }

        /// <summary>
        /// 获取功能树数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetTreeData()
        {
            var dtos = _departmentservice.GetAllList();
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
        /// <returns></returns>
        // GET api/v1/[controller]/GetChildrenByParent/1[?pageSize=3&pageIndex=10]
        [HttpGet]
        [Route("[action]/{departmentId}")]
        public IActionResult GetChildrenByParent(Guid departmentId, [FromQuery]int startPage, [FromQuery] int pageSize)
        {
            int rowCount = 0;
            var result = _service.GetChildrenByDepartment(departmentId, startPage, pageSize, out rowCount);
            var res = new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            };
            //string _json = JsonConvert.SerializeObject(res);
            return Ok(new
            {
                rowCount = rowCount,
                pageCount = Math.Ceiling(Convert.ToDecimal(rowCount) / pageSize),
                rows = result,
            });
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        // POST api/User
        [HttpPost]
        public async Task<IActionResult> Create([FromBody]UserDto dto)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            UserDto currUser=null; 
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
        // PUT api/User
        [HttpPut]
        public  IActionResult Edit([FromBody]UserDto dto)
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
        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="rpm"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("[action]")]
        public IActionResult ResetPassword([FromBody]ResetPasswordModel rpm)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
           
            UserDto cuser =  _service.Get(rpm.resetPasswordId);
            if (rpm.OldPassword != cuser.Password)
            {
                return Ok(new
                {
                    Result = "Faild",
                    Message = "原密码错误。"
                });
            }
            cuser.Password = rpm.NewPassword;

            if (_service.InsertOrUpdate(cuser,null))
            {
                return Ok(new { Result = "Success" });
            }
            return Ok(new { Result = "Faild" });
        }
        // DELETE api/User/DeleteMuti/5,6
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
        // DELETE api/User/5
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
        // GET api/User/5
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
