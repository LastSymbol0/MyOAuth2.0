using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Models;
using AuthServer.Service;
using AuthServer.Validation;
using AutoMapper;
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
        private IMapper Mapper;

        public ClientController(
            ClientsFirstTimeAccessHandler accessHandler,
            TokenManager tokenManager,
            IMapper mapper)
        {
            AccessHandler = accessHandler;
            TokenManager = tokenManager;
            Mapper = mapper;
        }

        [HttpPost("GetToken")]
        [RequiredHeadersAttribure("Content-Type")]
        public async Task<ActionResult<TokenResponceDTO>> GetToken()
        {
            Request.Headers.TryGetValue("Content-Type", out StringValues contentType);

            if (contentType == "application/x-www-form-urlencoded")
            {
                if (Request.Form.TryGetValue("grant_type", out StringValues grantType))
                {
                    if (grantType == "authorization_code")
                    {
                        var request = Mapper.Map<RequestTokenByCodeClientDTO>(Request.Form);

                        TokenResponceDTO token = AccessHandler.GetClientToken(request);

                        if (token == null)
                        {
                            return BadRequest("Access for your client was not found\nauthorization_code invalid");
                        }
                        return Ok(token);
                    }
                    else if (grantType == "refresh_token")
                    {
                        var request = Mapper.Map<RequestTokenRefreshClientDTO>(Request.Form);

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
            return BadRequest("Uuups...\nUnknown content type");
        }


        [Authorize]
        [HttpGet("Auth")]
        public async void Auth()
        {
            return;
        }
    }
}
