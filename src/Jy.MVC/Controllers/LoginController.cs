using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Jy.MVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Jy.MVCAuthorization;
using Jy.MVC.Services;
using Jy.Utility;


// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Jy.MVC.Controllers
{
    public class LoginController : JyControllerBase
    {
        private readonly ILoginService _loginSerivce;
        public LoginController( IOptionsSnapshot<UrlConfigSetting> urlConfig, ILoginService loginSerivce)
        {
            _loginSerivce = loginSerivce;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// 获取token后更新登录信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [BearerAuthorize]//token验证
        //[ValidateAntiForgeryToken]//csrf攻击，http://mt.sohu.com/it/d20170419/134928769_468635.shtml
        public IActionResult IndexRetrun()
        {
            var res = _loginSerivce.Index().Result;
            return Json(res);
        }

        /// <summary>
        /// 登陆获取token
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="role">角色</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken(LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            var token = await _loginSerivce.GetToken(model);
            if("Success".Equals(token.Result))
            {
                CookieOptions conf = new CookieOptions();
                conf.Expires = DateTimeOffset.Now.AddMinutes(30);
                HttpContext.Response.Cookies.Append("token", token.Token, conf);
            }
            return Json(token);
        }
        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyToken(string token)
        {
            var res = await _loginSerivce.VerifyToken(token);
            return Json(res);
        }
        [HttpPost]
        [BearerAuthorize]
        public async Task<IActionResult> LogOutToken(string token)
        {
            var res = await _loginSerivce.LogOutToken(token);
            if ("Success".Equals(res.Result))
            {
                HttpContext.Response.Cookies.Delete("token");
            }
            return Json(res);
        }
        [HttpGet]
        [BearerAuthorize]
        public async Task<IActionResult> GetMenuForLeftMenu(string token)
        {
            var res = await _loginSerivce.GetMenuForLeftMenu(token);
            return Json(res);
        }
         
    }
}
