using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ResourceServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ResourcesController : ControllerBase
    {
        private static readonly string[] Resources  = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool"
        };

        private readonly ILogger<ResourcesController> _logger;

        public ResourcesController(ILogger<ResourcesController> logger)
        {
            _logger = logger;
        }

        [Authorize(Policy = "Scope1")]
        [HttpGet("Get1")]
        public string Get1()
        {
            return Resources[0];
        }

        [Authorize(Policy = "Scope2")]
        [HttpGet("Get2")]
        public string Get2()
        {
            return Resources[1];
        }

        [Authorize(Policy = "Scope3")]
        [HttpGet("Get3")]
        public string Get3()
        {
            return Resources[2];
        }

        [Authorize(Policy = "Scope4")]
        [HttpGet("Get4")]
        public string Get4()
        {
            return Resources[3];
        }
    }
}
