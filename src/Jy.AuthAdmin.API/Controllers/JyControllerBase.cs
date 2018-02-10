using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Jy.Application.UserApp;
using Jy.Utility.Convert;
using Jy.AuthAdmin.API.Models;

namespace Jy.AuthAdmin.API.Controllers
{
    public abstract class JyControllerBase : ControllerBase
    {

        /// <summary>
        /// 获取服务端验证的第一条错误信息
        /// </summary>
        /// <returns></returns>
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
        /// <summary>
        /// 根据str获得list guid
        /// </summary>
        /// <param name="ids">ids</param>
        /// <param name="sp">分隔符</param>
        /// <returns></returns>
        protected List<Guid> GetList(string ids,char sp)
        {
            string[] idArray = ids.Split(sp);
            List<Guid> Ids = new List<Guid>();
            int? j = idArray?.Length;
            for (int i = 0; i < j; i++)
                Ids.Add(new Guid(idArray?[i]));
            return Ids;
            //string[] idArray = ids.Split(sp);
            //List<Guid> Ids = new List<Guid>();
            //foreach (string id in idArray)
            //{
            //    Ids.Add(Guid.Parse(id));
            //}
            //return Ids;
        }
    }
}
