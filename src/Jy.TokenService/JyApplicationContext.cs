using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Jy.TokenService
{
    public class JyApplicationContext
    {
        //跟在每个上下文中
        private static readonly AsyncLocal<string> ApplicationContext = new AsyncLocal<string>();

        public static void SaveApplicationContext(string applicationContext)
        {
            ApplicationContext.Value = applicationContext;
        }

        public static string GetApplicationContextFromAsyncLocal()
        {
            return ApplicationContext.Value;
        }
    }
}
