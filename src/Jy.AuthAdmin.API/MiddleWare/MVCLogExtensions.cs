using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;

namespace Jy.AuthAdmin.API.Middleware
{
    public static class MVCLogExtensions
    {
        public static IApplicationBuilder UseAuthLog(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseMiddleware<MVCLogMiddleWare>();
        }

        public static IApplicationBuilder UseAuthLog(this IApplicationBuilder builder, MVCLogOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return builder.UseMiddleware<MVCLogMiddleWare>(Options.Create(options));
        }
    }
}