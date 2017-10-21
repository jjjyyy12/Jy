using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Jy.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Jy.MVCAuthorization;
using Jy.MVC.Services;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Jy.MVC.Controllers
{
    [BearerAuthorize]
    public class UserController : JyControllerBase
    {
        private readonly IUserService _service;
        public UserController(IUserService service)
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
        public async Task<IActionResult> GetTreeData()
        {
            var res = await _service.GetTreeData();
            return Json(res);
        }
        /// <summary>
        /// 获取子级列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetChildrenByParent(Guid departmentId, int startPage, int pageSize)
        {
            var res = await _service.GetChildrenByParent(departmentId, startPage, pageSize);
            return Json(res);
        }
        /// <summary>
        /// 新增或编辑功能
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(Models.User dto)
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
            else {
                var res = await _service.Edit(dto);
                return Json(res);
            } //edit
           
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel rpm)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }

            var res = await _service.ResetPassword(rpm);
            return Json(res);
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
