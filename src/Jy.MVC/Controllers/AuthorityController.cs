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
    [BearerAuthorize]
    public class AuthorityController : JyControllerBase
    {
        private readonly IAuthorityService _service;
        public AuthorityController(IAuthorityService service)
        {
            _service = service;
        }

        // GET: /<controller>/
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRoleTreeData(Guid id)
        {
            var res = await _service.GetRoleTreeData(id);
            return Json(res);
        }
        [HttpGet]
        public async Task<IActionResult> GetBatchRoleTreeData()
        {
            var res = await _service.GetBatchRoleTreeData();
            return Json(res);
        }
        [HttpPost]
        public async Task<IActionResult> BatchUserRole(BatchUserRoleModel rpm)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            var res = await _service.BatchUserRole(rpm);
            return Json(res);
        }
        [HttpPost]
        public async Task<IActionResult> UserRole(UserRoleModel rpm)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            var res = await _service.UserRole(rpm);
            return Json(res);
        }
        
    }
}
