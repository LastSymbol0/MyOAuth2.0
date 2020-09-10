using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Mediator;
using AuthServer.Mediator.Commands;
using AuthServer.Mediator.Queries;
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
        private IMediator Mediator;

        public ClientController(IMediator mediator)
        {
            Mediator = mediator;
        }


        [HttpPost("GetToken")]
        [RequiredHeadersAttribure("Content-Type")]
        public async Task<ActionResult<GetTokenPairQueryDTO>> GetToken(
                                [FromForm(Name = "grant_type")] String grantType,
                                [FromForm(Name = "refresh_token")] String refreshToken,
                                [FromForm(Name = "redirect_url")] String redirectUrl,
                                [FromForm(Name = "code")] String code,
                                [FromHeader(Name = "Authorization")] string authHeader)
        {
            // Tmp client auth
            // TODO: Client's authorization header value must be parsed in auth middleware
            //       and accessed here via something like User.Id
            var (clientId, clientSecret) = Utils.Utils.GetLoginPasswordFromAuthHeader(authHeader);

            SessionCommandResponce commandResponce;

            if (grantType == "authorization_code")
            {
                commandResponce = await Mediator.Execute<AuthenticateClientCommand, SessionCommandResponce>(new AuthenticateClientCommand
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    AuthenticationCode = code
                });

            }
            else if (grantType == "refresh_token")
            {
                commandResponce = await Mediator.Execute<RefreshClientTokenCommand, SessionCommandResponce>(new RefreshClientTokenCommand
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret,
                    RefreshToken = refreshToken
                });
            }
            else
            {
                return BadRequest("Invalid Grant Type");
            }

            if (!commandResponce.IsSucceed)
            {
                return BadRequest("Access for your client was not found\nGrant may be invalid");
            }

            var resp = await Mediator.Get<GetTokenPairQuery, GetTokenPairQueryDTO>(new GetTokenPairQuery
            {
                SessionId = commandResponce.SessionId ?? 0 // here commandResponce.SessionId will never be null
            });

            return Ok(resp);
        }


        [Authorize]
        [HttpGet("Auth")]
        public async void Auth()
        {
            return;
        }
    }
}
