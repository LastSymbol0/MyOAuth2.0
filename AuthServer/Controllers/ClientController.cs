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
        private readonly ClientsFirstTimeAccessHandler AccessHandler;
        private TokenManager TokenManager;

        public ClientController(ClientsFirstTimeAccessHandler accessHandler, TokenManager tokenManager)
        {
            AccessHandler = accessHandler;
            TokenManager = tokenManager;
        }

        [HttpPost("GetToken")]
        public async Task<ActionResult<TokenResponceDTO>> GetToken()
        {
            if (Request.Headers.TryGetValue("Content-Type", out StringValues contentType))
            {
                if (contentType == "application/x-www-form-urlencoded")
                {
                    if (Request.Form.TryGetValue("grant_type", out StringValues grantType))
                    {
                        if (grantType == "authorization_code")
                        {
                            var request = new RequestTokenByCodeClientDTO(Request.Form);

                            TokenResponceDTO token = AccessHandler.GetClientToken(request);

                            if (token == null)
                            {
                                return BadRequest("Access for your client was not found\nauthorization_code invalid");
                            }
                            return Ok(token);
                        }
                        else if (grantType == "refresh_token")
                        {
                            var request = new RequestTokenRefreshClientDTO(Request.Form);

                            TokenResponceDTO token = TokenManager.GenerateTokenPair(request.RefreshToken);

                            if (token == null)
                            {
                                return BadRequest("Access for your client was not found\refresh_token invalid");
                            }
                            return Ok(token);
                        }
                    }
                    else
                    {
                        throw new ArgumentNullException("grant_type value missed.");
                    }
                }
                else if (contentType == "application/json")
                {
                    return BadRequest("Uuups...\nUnsupported content type");
                }
                else
                {
                    return BadRequest("Uuups...\nUnknown content type");
                }
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
