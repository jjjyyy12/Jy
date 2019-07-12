using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.IO;
using System.Threading.Tasks;

namespace Jy.AuthAdmin.API.Middleware
{
    public class MVCLogMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly MVCLogOptions _options;
        private readonly ILogger _logger;
        public MVCLogMiddleWare(RequestDelegate next, IOptions<MVCLogOptions> options, ILoggerFactory loggerFactory)
        {
            _next = next;
            _options = options.Value;
            _logger = loggerFactory.CreateLogger<MVCLogMiddleWare>();
        }

        public async Task Invoke(HttpContext context)
        {
            string isLogAccessInfo = "True";//Configs.IsLogAccessInfo ?? "False";
            if ("True".Equals(isLogAccessInfo))
            {
                string url = context.Request.Host.Value + context.Request.Path.Value;
                string method = context.Request.Method;

                context.Request.EnableBuffering();
                var requestReader = new StreamReader(context.Request.Body);

                var requestContent = requestReader.ReadToEnd();
                _logger.LogInformation($"{url},{method},begin:{requestContent}");

                context.Request.Body.Position = 0;
                var org = context.Response.Body;
                using (var ms = new MemoryStream())
                {
                    context.Response.Body = ms;
                    await _next.Invoke(context);

                    ms.Seek(0, SeekOrigin.Begin);
                    var responseReader = new StreamReader(ms);

                    var responseContent = responseReader.ReadToEnd();
                    _logger.LogInformation($"{url},{method},end:{responseContent}");

                    ms.Seek(0, SeekOrigin.Begin);
                    await ms.CopyToAsync(org);
                    context.Response.Body = org;
                }
            }
            else
            {
                await _next.Invoke(context);
            }

        }
    }
}
