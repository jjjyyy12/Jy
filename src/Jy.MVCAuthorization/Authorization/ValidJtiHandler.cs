
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Jy.Utility;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using Jy.ServicesKeep;

namespace Jy.MVCAuthorization
{
    //api工程中统一引用的鉴权代码
    public class ValidJtiHandler : AuthorizationHandler<ValidJtiRequirement>
    {
        private readonly IOptionsSnapshot<UrlConfigSetting> _urlConfig;

        public ValidJtiHandler(IOptionsSnapshot<UrlConfigSetting> urlConfig)
        {
            _urlConfig = urlConfig;
        }

        //验证当前请求的角色权限
        private string VerfiyAuth(string jti,string userId,string roleIds,string currController,string currPath)
        {
            HttpClient _httpClient = new HttpClient();
            var parameters = new Dictionary<string, string>();
            parameters.Add("jti", jti);
            parameters.Add("userId", userId);
            parameters.Add("roleIds", roleIds);
            parameters.Add("currController", currController);
            parameters.Add("currPath", currPath);
            parameters.Add("role", "validate");
            var response =  _httpClient.PostAsync($"{KeepCallServer.getTokenAuthHostPort(_urlConfig.Value.ZooKeeperList)}{_urlConfig.Value.TokenAuthUrl}", new FormUrlEncodedContent(parameters)).Result; //验证权限需同步执行
            var result =  response.Content.ReadAsStringAsync();
            if (response.StatusCode != HttpStatusCode.OK)
                return result.Result;
            var json = JsonConvert.DeserializeObject<Dictionary<string, object>>(result.Result);
            return json.Where(t => t.Key == "Result").FirstOrDefault().Value.ToString();
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ValidJtiRequirement requirement)
        {
            // 检查 Jti 是否存在
            var jti = context.User.FindFirst("jti")?.Value;
            if (jti == null)
            {
                context.Fail(); // 显式的声明验证失败
                return Task.CompletedTask;
            }

            var mvcContext = context.Resource as Microsoft.AspNetCore.Mvc.Filters.AuthorizationFilterContext;
            if (mvcContext != null)
            {
                // Examine MVC specific things like routing data.
                var claims = context.User.Claims.ToList();
                var roleIds = claims.Find(t => ClaimTypes.Role.Equals(t.Type)).Value;
                var userId = claims.Find(t => ClaimTypes.NameIdentifier.Equals(t.Type)).Value;
                var currController = mvcContext.RouteData.Values["controller"].ToString();
                var requestType = mvcContext.HttpContext.Request.Method;
                var currPath = mvcContext.HttpContext.Request.Path.Value;
                if (currPath.IndexOf("/Login") < 0)
                {
                    var res = VerfiyAuth(jti, userId, roleIds, currController, currPath);
                    if (!"Success".Equals(res)) //验证当前请求是否有权限
                    {
                        context.Fail();
                        return Task.CompletedTask;
                    }
                }
                    
            }
            context.Succeed(requirement); // 显式的声明验证成功
            return Task.CompletedTask;
        }
    }
}
