using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Jy.TokenService;

namespace Jy.TokenAuth.Middleware
{

    public class TokenProviderMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly TokenProviderOptions _options;
        private IVerifyTokenAppService _verifyTokenAppService;
        public TokenProviderMiddleware(RequestDelegate next,IOptions<TokenProviderOptions> options, IVerifyTokenAppService verifyTokenAppService)
        {
            _next = next;
            _options = options.Value;
            _verifyTokenAppService = verifyTokenAppService;
        }

        public Task Invoke(HttpContext context)
        {
            // If the request path doesn't match, skip
            if (!context.Request.Path.Equals(_options.Path, StringComparison.Ordinal))
            {
                //use new JwtSecurityTokenHandler().ValidateToken() to valid token
                return _next(context);
            }

            // Request must be POST with Content-Type: application/x-www-form-urlencoded
            if (!context.Request.Method.Equals("POST")
              || !context.Request.HasFormContentType)
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }

            var role = context.Request.Form["role"];
            if ("black".Equals(role)) //注销
                return BlackToken(context);
            else if ("auth".Equals(role)) //生成token
                return GenerateToken(context);
            else if ("validate".Equals(role))//验证tokenrole
                return ValidateRole(context);
            else if ("validatetoken".Equals(role))//验证token
                return ValidateToken(context);
            else
            {
                context.Response.StatusCode = 400;
                return context.Response.WriteAsync("Bad request.");
            }
        }

        private async Task GenerateToken(HttpContext context)
        {
            var username = context.Request.Form["username"];
            var password = context.Request.Form["password"];

            try { 
                var user = _verifyTokenAppService.CheckUser(username, password);
          
                if (user == null)
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid username or password.");
                    return;
                }

                var now = DateTime.UtcNow;
                var jti = Guid.NewGuid().ToString();
                // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
                // You can add other claims here, if you want:
                var claims = new Claim[]
                {
                    new Claim(ClaimTypes.Role, string.Join(",", user.RoleIds) ?? string.Empty), // 添加角色信息
                    new Claim(ClaimTypes.NameIdentifier,user.Id.ToString() ),
                    new Claim(JwtRegisteredClaimNames.NameId, username),
                    new Claim(JwtRegisteredClaimNames.Jti, jti),
                    new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(now).ToString(), ClaimValueTypes.Integer64)
                };

                // Create the JWT and write it to a string
                var jwt = new JwtSecurityToken(
                  issuer: _options.Issuer,
                  audience: _options.Audience,
                  claims: claims,
                  notBefore: now,
                  expires: now.Add(_options.Expiration),
                  signingCredentials: _options.SigningCredentials);
                var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

                var response = new
                {
                    access_token = encodedJwt,
                    expires_in = (int)_options.Expiration.TotalSeconds
                };
                _verifyTokenAppService.SaveToken(user, encodedJwt,jti, _options.Expiration);
                // Serialize and return the response
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
            }
            catch (Exception ex)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync($"identity failed:{ex.Message}");
                return;
            }

        }
        private async Task BlackToken(HttpContext context)
        {
            var oldToken = context.Request.Form["oldToken"];
            _verifyTokenAppService.BlackToken(oldToken,_options.Expiration);
            var response = new
            {
                Result = "Success",
                expires_in = (int)_options.Expiration.TotalSeconds
            };
            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }
        private async Task ValidateRole(HttpContext context)
        {
            var jti = context.Request.Form["jti"];
            var userId = context.Request.Form["userId"];
            var roleIds = context.Request.Form["roleIds"];
            var currController = context.Request.Form["currController"];
            var currPath = context.Request.Form["currPath"];

            string resultStr = "Failed";
            var blacktokenExists = _verifyTokenAppService.VerifyBlackRecordsToken(jti);// _userService.BlackRecords.Any(r => r.Jti == jti);
            if (!blacktokenExists)
                if (_verifyTokenAppService.VerifyTokenRole(userId, roleIds))
                    if (currPath.ToString().IndexOf("/Login") < 0)
                        if (_verifyTokenAppService.VerifyCurrActionRole(userId, currController)) //验证当前请求是否有权限
                            resultStr = "Success";

            var response = new
            {
                Result = resultStr
            };
            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        private async Task ValidateToken(HttpContext context)
        {
            var token = context.Request.Form["token"];

            string resultStr = "Failed";
            if(_verifyTokenAppService.VerifyToken(token))
                resultStr = "Success";

            var response = new
            {
                Result = resultStr
            };
            // Serialize and return the response
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
        }

        public static long ToUnixEpochDate(DateTime date) => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);

    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class TokenProviderMiddlewareExtensions
    {
        public static IApplicationBuilder UseTokenProviderMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TokenProviderMiddleware>();
        }
    }


}
