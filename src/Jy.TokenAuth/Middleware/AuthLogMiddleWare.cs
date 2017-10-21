using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Jy.TokenAuth.Middleware
{
    public class AuthLogMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly AuthLogOptions _options;
        private readonly ILogger _logger;
        public AuthLogMiddleWare(RequestDelegate next, IOptions<AuthLogOptions> options, ILoggerFactory loggerFactory)
        {
            this._next = next;
            this._options = options.Value;
            _logger = loggerFactory.CreateLogger<AuthLogMiddleWare>();
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation("Begin request: " + context.Request.Path);
            await _next.Invoke(context);
            _logger.LogInformation("End request: " + context.Request.Path);
        }
    }
}
