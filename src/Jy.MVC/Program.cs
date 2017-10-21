using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Jy.ServicesKeep;
#if NET461
using Microsoft.AspNetCore.Hosting.WindowsServices;
#endif
namespace Jy.MVC
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()//.UseUrls("http://*:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .Build();

#if NET461
            if (args.Contains("--windows-service"))
            {
                host.RunAsService();
            }
            else
            {
                host.Run();
            }
#else
            host.Run();
#endif


        }

        
    }
}
