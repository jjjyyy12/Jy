using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;

namespace Jy.HealthCheck
{
    public static class HealthCheckExtensions
    {
        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseMiddleware<HealthCheckMiddleware>();
        }

        public static IApplicationBuilder UseHealthCheck(this IApplicationBuilder builder, string path,int? port,TimeSpan timeout)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (port.HasValue)
            {
               return builder.UseMiddleware<HealthCheckMiddleware>(port, timeout);
            }
            else
            {
               return builder.UseMiddleware<HealthCheckMiddleware>(path, timeout);
            }
        }
    }
}