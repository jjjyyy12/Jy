﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace Jy.Dapper
{
    public class DapperMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly DapperOptions _options;

        public DapperMiddleWare(RequestDelegate next, IOptions<DapperOptions> options)
        {
            this._next = next;
            this._options = options.Value;
        }

        public async Task Invoke(HttpContext context)
        {

            await _next.Invoke(context);
        }
    }
}
