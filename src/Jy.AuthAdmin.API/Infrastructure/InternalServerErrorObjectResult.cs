using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jy.AuthAdmin.API.Infrastructure
{
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object error)
            : base(error)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}
