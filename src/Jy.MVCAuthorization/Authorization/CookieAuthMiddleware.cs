using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Jy.MVCAuthorization
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class CookieAuthMiddleware
    {
        public static CookieAuthenticationOptions GetOptions()
        {
            return new CookieAuthenticationOptions
            {
                //AutomaticAuthenticate = true,
                //AutomaticChallenge = true,
                LoginPath = new PathString("/login"),
                LogoutPath = new PathString("/logout"),
                AccessDeniedPath = new PathString("/test"),
                CookieHttpOnly = false,  //默认就是True了 
                CookieName = "jy_access_token",
                SlidingExpiration = true,
                CookieManager = new ChunkingCookieManager()
            };
        }
    }
    public static class IdentityExtension
    {
        public static string FullName(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("name");
            return (claim != null) ? claim.Value : string.Empty;
        }
        public static string Role(this IIdentity identity)
        {
            var claim = ((ClaimsIdentity)identity).FindFirst("role");
            return (claim != null) ? claim.Value : string.Empty;
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class CookieAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseCookieAuthMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CookieAuthMiddleware>();
        }
    }
}
