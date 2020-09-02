using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Models;
using AuthServer.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace AuthServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly ClientsAccessHandler AccessHandler;
        public ClientController(ClientsAccessHandler accessHandler)
        {
            AccessHandler = accessHandler;
        }

        [HttpPost("GetToken")]
        public async Task<ActionResult<string>> GetToken()
        {
            if (Request.Headers.TryGetValue("Content-Type", out StringValues contentType))
            {
                RequestTokenClientDTO client = new RequestTokenClientDTO();

                if (contentType == "application/x-www-form-urlencoded")
                {
                    client.FromForm(Request.Form);
                }
                else if (contentType == "application/json")
                {
                    return BadRequest("Uuups...\nUnsupported content type");
                }
                else
                {
                    return BadRequest("Uuups...\nUnknown content type");
                }

                string token = AccessHandler.GetClientToken(client);

                if (String.IsNullOrEmpty(token))
                {
                    return BadRequest("Access for your client was not found\n");
                }

                return Ok(token);
            }
            return BadRequest("Missing Content-Type header");
        }

        [Authorize]
        [HttpGet("Auth")]
        public async void Auth()
        {
            return;
        }
    }
}
