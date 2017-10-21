using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Jy.Utility.Convert;
using Jy.MVC.Models;

namespace Jy.MVC.Controllers
{
    public abstract class JyControllerBase : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // byte[] result;
            //filterContext.HttpContext.Session.TryGetValue("token", out result);//CurrentUser
            string result;
            filterContext.HttpContext.Request.Cookies.TryGetValue("token", out result);
            string path = filterContext.HttpContext.Request.Path;
            if (result == null &&path.IndexOf("/Login/")<0)
            {
                filterContext.Result = new RedirectResult("/Login/Index");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 获取服务端验证的第一条错误信息
        /// </summary>
        protected string GetModelStateError()
        {
            foreach (var item in ModelState.Values)
            {
                if (item.Errors.Count > 0)
                {
                    return item.Errors[0].ErrorMessage;
                }
            }
            return "";
        }
    }
}
