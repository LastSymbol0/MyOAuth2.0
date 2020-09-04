using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Validation
{
    public class RequiredHeadersAttribure : ActionFilterAttribute
    {
        private readonly string[] Headers;

        public RequiredHeadersAttribure(string header)
        {
            Headers = new string[] { header };
        }

        public RequiredHeadersAttribure(string[] headers)
        {
            Headers = headers;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            foreach (var header in Headers)
            {
                if (!context.HttpContext.Request.Headers.ContainsKey(header))
                {
                    context.Result = new BadRequestObjectResult($"Missing {header} required header");
                }
            }
        }
    }
}
