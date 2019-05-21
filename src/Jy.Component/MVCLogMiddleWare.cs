using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Jy.Component
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
            _logger.LogInformation("Begin request: " + context.Request.Path);
            var headers = context.Request.Headers;
         
            await _next.Invoke(context);
            _logger.LogInformation("End request: " + context.Request.Path);
        }
    }
}
