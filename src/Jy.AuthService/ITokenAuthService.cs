using Jy.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jy.AuthService
{
    /// <summary>
    /// 负责各个api与tokenauth（identityserver）的相关调用封装
    /// </summary>
    public interface ITokenAuthService
    {
        /// <summary>
        /// 获取token令牌
        /// </summary>
        /// <param name="username">username</param>
        /// <param name="password">password</param>
        /// <param name="role">black： 注销 auth： 生成token validate： 验证tokenrole  validatetoken：验证token</param>
        /// <param name="tokenServerURL">identityserverurl</param>
        /// <returns></returns>
        Task<string> GetToken(string username, string password, string role, string tokenServerURL);
        /// <summary>
        /// 验证token令牌
        /// </summary>
        /// <param name="token">token</param>
        /// <param name="role">black： 注销 auth： 生成token validate： 验证tokenrole  validatetoken：验证token</param>
        /// <param name="tokenServerURL">identityserverurl</param>
        /// <returns></returns>
        Task<string> VerifyToken(string token, string role, string tokenServerURL);
        /// <summary>
        ///注销令牌，jwt加入黑名单
        /// </summary>
        /// <param name="oldToken">失效token</param>
        /// <param name="role">black： 注销 auth： 生成token validate： 验证tokenrole  validatetoken：验证token</param>
        /// <param name="tokenServerURL">identityserverurl</param>
        /// <returns></returns>
        Task<string> BlackToken(string oldToken, string role, string tokenServerURL);
    }
}
