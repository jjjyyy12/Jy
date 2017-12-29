using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Jy.AuthAdmin.API.Models;
using Microsoft.AspNetCore.Authorization;

using Jy.TokenService;
using Microsoft.Extensions.Options;
using Jy.MVCAuthorization;
using Microsoft.AspNetCore.Authentication;
using Jy.Utility;
using Jy.ServicesKeep;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Jy.AuthAdmin.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    public class LoginController : JyControllerBase
    {
        private IVerifyTokenAppService _verifyTokenAppService;
        private ITokenAuthService _tokenAuthService; //调用tokenauth服务相关
        private readonly IOptions<UrlConfigSetting> _urlConfig;
        private IHttpContextAccessor _httpContextAccesor;
        public LoginController( IVerifyTokenAppService verifyTokenAppService,
            ITokenAuthService tokenAuthService,
            IOptions<UrlConfigSetting> urlConfig,
            IHttpContextAccessor httpContextAccesor)
        {
            _verifyTokenAppService = verifyTokenAppService;
            _tokenAuthService = tokenAuthService;
            _urlConfig = urlConfig;
            _httpContextAccesor = httpContextAccesor;
        }
 
        /// <summary>
        /// 获取token后更新登录信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [BearerAuthorize]//token验证
        //[ValidateAntiForgeryToken]//csrf攻击，http://mt.sohu.com/it/d20170419/134928769_468635.shtml
        public async Task<IActionResult> Index()
        {
            var token = GetUserTokenAsync().Result;
            if (!string.IsNullOrWhiteSpace(token))
            {
                //检查用户信息
                var user = _verifyTokenAppService.GetCurrentUserStatus(token);
                if (user != null)
                {
                    //记录最近登录时间
                    await _verifyTokenAppService.Login(user.UserId);

                    return Ok(new { Result = "Success"});
                }
                return Ok(new { Result = "Faild", Message = "No user info"  });
            }else
            return Ok(new
            {
                Result = "Faild",
                Message = "Not login"
            });
        }

        /// <summary>
        /// 登陆获取token
        /// </summary>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="role">角色</param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> GetToken([FromBody]LoginModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok(new
                {
                    Result = "Faild",
                    Message = GetModelStateError()
                });
            }
            var token = await _tokenAuthService.GetToken(model.UserName, model.Password,"auth", $"{KeepCallServer.getTokenAuthHostPort(_urlConfig.Value.ZooKeeperList)}{_urlConfig.Value.TokenAuthUrl}");
            if (!string.IsNullOrWhiteSpace(token)&&token.Length>50)
            {
                CookieOptions conf = new CookieOptions();
                conf.Expires = DateTimeOffset.Now.AddMinutes(30);
                //HttpContext.Response.Cookies.Append("token", token, conf);
                //HttpContext.Session.Set("token", ByteConvertHelper.Object2Bytes(token));
                return Ok(new { Result = "Success", Token = token });
            }
            else
            {
                return Ok(new { Result = "Faild" ,Message= token });
            }
        }
        /// <summary>
        /// 验证token
        /// </summary>
        /// <param name="token">token</param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyToken([FromBody]string token)
        {
            var res = await _tokenAuthService.VerifyToken(token, "validatetoken", $"{KeepCallServer.getTokenAuthHostPort(_urlConfig.Value.ZooKeeperList)}{_urlConfig.Value.TokenAuthUrl}");
            if ("Success".Equals(res))
            {
                //HttpContext.Response.Cookies.Delete("token");
                //HttpContext.Session.Remove("CurrentUser");
                return Ok(new { Result = "Success", Token = token });
            }
            else
            {
                return Ok(new { Result = "Faild" });
            }
        }
        /// <summary>
        /// 登出
        /// </summary>
        /// <param name="token">token</param>
        /// <returns></returns>
        [HttpPost]
        [Route("[action]")]
        [BearerAuthorize]
        public async Task<IActionResult> LogOutToken([FromBody]string token)
        {
            var res = await _tokenAuthService.BlackToken(token, "black", $"{KeepCallServer.getTokenAuthHostPort(_urlConfig.Value.ZooKeeperList)}{_urlConfig.Value.TokenAuthUrl}");
            if ("Success".Equals(res))
            {
                //HttpContext.Response.Cookies.Delete("token");
                //HttpContext.Session.Remove("CurrentUser");
                return Ok(new { Result = "Success", Token = token });
            }
            else
            {
                return Ok(new { Result = "Faild" });
            }
        }
        /// <summary>
        /// 得到左边的菜单信息
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("[action]")]
        [BearerAuthorize]
        public IActionResult GetMenuForLeftMenu(string token)
        {
            if(string.IsNullOrWhiteSpace(token)) return  NotFound();
            var roleMenus = _verifyTokenAppService.GetRowMenuForLeftMenu(token);
            var userStatus = _verifyTokenAppService.GetCurrentUserStatus(token);
            return Ok(new { Menus = roleMenus
                , UserName = userStatus.UserName 
                ,Name = userStatus.Name
                ,DepartmentName = userStatus.DepartmentName
                , LoginTime = userStatus.LoginTime.ToString("yyyy-MM-dd HH:mm:ss")
                ,Email = userStatus.Email
                ,Mobile = userStatus.Mobile});
        }


        async Task<string> GetUserTokenAsync()
        {
            //var context = _httpContextAccesor.HttpContext;
            //return await context.Authentication.GetTokenAsync("access_token");
            return await _httpContextAccesor.HttpContext.GetTokenAsync("access_token");
        }
    }
}
