using Jy.Domain.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Jy.TokenService
{
    /// <summary>
    /// 用户登录、验证用户权限，原则上只被TokenAuth（identityserver），authadminapi（用户，权限系统）引用
    /// </summary>
    public interface IVerifyTokenAppService
    {
        //验证令牌
        bool VerifyToken(string token);
        //验证当前token的附带的role信息
        bool VerifyTokenRole(string userId, string roleIds);
        //验证此次请求是否符合有权限访问action
        bool VerifyCurrActionRole(string userId, string currController);
        //获取左侧菜单
        List<RoleMenuDto> GetRowMenuForLeftMenu(string token);
        //jwt 是否注销了
        bool VerifyBlackRecordsToken(string jti);
        //保存令牌
        void SaveToken(UserDto dto, string token, string jti, TimeSpan expires);
        //注销令牌的实质操作
        void BlackToken(string token, TimeSpan expires);
        //获取当前用户状态信息
        UserToken GetCurrentUserStatus(string token);
        //验证用户
        UserDto CheckUser(string userName, string password);
        //修改登陸時間
        Task<bool> Login(Guid id);
    }
}
