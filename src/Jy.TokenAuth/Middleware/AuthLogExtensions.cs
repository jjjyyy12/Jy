using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using System;

namespace Jy.TokenAuth.Middleware
{
    public static class AuthLogExtensions
    {
        public static IApplicationBuilder UseAuthLog(this IApplicationBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.UseMiddleware<AuthLogMiddleWare>();
        }

        public static IApplicationBuilder UseAuthLog(this IApplicationBuilder builder, AuthLogOptions options)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            return builder.UseMiddleware<AuthLogMiddleWare>(Options.Create(options));
        }
    }
}