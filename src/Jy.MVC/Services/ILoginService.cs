using Jy.MVC.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Jy.MVC.Services
{
    public interface ILoginService
    {
        //登录后跳转Index
        Task<ReturnObj> Index();
        //获取令牌
        Task<TokenRes> GetToken(LoginModel model);
        //获取令牌
        Task<TokenRes> VerifyToken(string token);
        //注销
        Task<TokenRes> LogOutToken(string token);

        Task<LeftMenu> GetMenuForLeftMenu(string token);
       
    }
}
