using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Jy.Application.RoleApp;
using Jy.AuthAdmin.API.Models;
using Jy.Application.UserApp;
using Jy.MVCAuthorization;
using Jy.Domain.Dtos;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Jy.AuthAdmin.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [BearerAuthorize]
    public class AuthorityController : JyControllerBase
    {
        private readonly IRoleAppService _service;
        private readonly IUserAppService _userAppService;
        public AuthorityController(IRoleAppService service , IUserAppService userAppService)
        {
            _service = service;
            _userAppService = userAppService;
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="id">user id</param>
        /// <returns></returns>
        // GET api/v1/[controller]/GetRoleTreeData/id
        [HttpGet]
        [Route("[action]/{id}")]
        public IActionResult GetRoleTreeData(Guid id)
        {
            var roles = _service.GetAllList();
            var userRoles = _userAppService.GetUserRoles(id);
            List<TreeCheckBoxModel> treeModels = new List<TreeCheckBoxModel>();

            roles.ForEach((role) =>
            {
                UserRoleDto tempt = userRoles.Find((userRole) => role.Id == userRole.RoleId);
                treeModels.Add(new TreeCheckBoxModel() { Id = role.Id.ToString(), Text = role.Name, Parent =  "#", Checked = tempt!=null?"1":"0" });
            });

            return Ok(treeModels);
        }
        /// <summary>
        /// 得到功能树
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        public IActionResult GetBatchRoleTreeData()
        {
            var roles = _service.GetAllList();
            List<TreeCheckBoxModel> treeModels = new List<TreeCheckBoxModel>();

            roles.ForEach((role) =>
            {
                treeModels.Add(new TreeCheckBoxModel() { Id = role.Id.ToString(), Text = role.Name, Parent = "#", Checked = "0" });
            });

            return Ok(treeModels);
        }
        /// <summary>
        /// 批量修改用户角色
        /// </summary>
        /// <param name="rpm">BatchUserRoleModel</param>
        /// <returns></returns>
        // PUT api/Authority/Batch
        [HttpPut]
        [Route("Batch")]
        public IActionResult BatchUserRole([FromBody]BatchUserRoleModel rpm)
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
                string[] userIds = rpm.userIDs?.Split('_');
                string[] roleIds = rpm.roleIDs?.Split('_');
                List<Guid> uIds = new List<Guid>();
                List<Guid> rIds = new List<Guid>();
                foreach (string rid in roleIds)
                {
                    rIds.Add(Guid.Parse(rid));
                }
                foreach (string id in userIds)
                {
                    uIds.Add(Guid.Parse(id));
                }

                _userAppService.BatchUpdateUserRoles(uIds, rIds);
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
        /// 修改用户角色
        /// </summary>
        /// <param name="rpm">UserRoleModel</param>
        /// <returns></returns>
        // PUT api/Authority
        [HttpPut]
        public IActionResult UserRole([FromBody]UserRoleModel rpm)
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
                string[] roleIDs = rpm.roleIDs?.Split('_');
                List<Guid> dto = new List<Guid>();
                int? j = roleIDs?.Length;

                for (int  i = 0  ; i < j; i++)
                    dto.Add( new Guid(roleIDs?[i]));

                _userAppService.UpdateUserRoles(rpm.userRoleId, dto);

                return Ok(new { Result = "Success" });
            }
            catch(Exception ex)
            {
                return Ok(new { Result = "Faild" , Message = ex .Message});
            }
        }
 
    }
}
