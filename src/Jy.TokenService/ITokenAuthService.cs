using Jy.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jy.TokenService
{
    public interface ITokenAuthService
    {
        //获取token令牌
        Task<string> GetToken(string username, string password, string role, string tokenServerURL);
        //验证token令牌
        Task<string> VerifyToken(string token, string role, string tokenServerURL);
        //注销令牌，jwt加入黑名单
        Task<string> BlackToken(string oldToken, string role, string tokenServerURL);
    }
}
