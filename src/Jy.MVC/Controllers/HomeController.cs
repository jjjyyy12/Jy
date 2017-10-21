using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Jy.Application.Token;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Jy.MVC.Controllers
{
    public class HomeController : JyControllerBase
    {
        // GET: /<controller>/
        public IActionResult Index()
        {
            //throw new Exception("异常");
            return View();
        }
    }
}
