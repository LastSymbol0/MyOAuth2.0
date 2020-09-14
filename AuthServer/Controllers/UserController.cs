using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Domain.AggregatesModel.SessionAggregate;
using AuthServer.Mediator;
using AuthServer.Mediator.Commands;
using AuthServer.Mediator.Queries;
using AuthServer.Models;
using AuthServer.Models.GivingAccessPage;
using AuthServer.Service;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    public class UserController : Controller
    {
        private readonly IMediator Mediator;
        private readonly IMapper Mapper;

        public UserController(IMediator mediator, IMapper mapper)
        {
            Mediator = mediator;
            Mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAccessToClient([FromQuery(Name = "clientId")] string clientId,
                                                [FromQuery(Name = "responceType")] string responceType,
                                                [FromQuery(Name = "redirectUrl")] string redirectUrl,
                                                [FromQuery(Name = "state")] string state,
                                                [FromHeader(Name = "Authorization")] string authHeader)
        {
            // Tmp resource owner auth
            // TODO: Resource owner's authorization header value must contain token and
            //       it must be parsed in auth middleware and accessed here via something like User.Id
            var (login, pass) = Utils.Utils.GetLoginPasswordFromAuthHeader(authHeader);

            RequestCodeClientDTO info = new RequestCodeClientDTO();

            info.ResouceOwnerId = login;
            info.ClientId = clientId;
            info.ResponceType = responceType;
            info.RedirectUrl = redirectUrl;
            info.State = state;

            return View(new GivingAccessModel{ ClientInfo = info, AccessParameters = new AccessParametersDTO()});
        }

        [HttpPost]
        public async Task<IActionResult> GetAccessToClientAsync(GivingAccessModel model)
        {
            SessionCommandResponce commandResponce = await Mediator.Execute<StartSessionCommand, SessionCommandResponce>(new StartSessionCommand
            {
                ClientId = model.ClientInfo.ClientId,
                ResourceOwnerId = model.ClientInfo.ResouceOwnerId,
                ClientSecret = "",
                AccessParameters = Mapper.Map<AccessParameters>(model.AccessParameters)
            });

            if (commandResponce.IsSucceed)
            {
                var code = await Mediator.Get<GetClientAuthCodeQuery, string>(new GetClientAuthCodeQuery
                {
                    SessionId = (int)commandResponce.SessionId
                });

                return Redirect($"{model.ClientInfo.RedirectUrl}?state={model.ClientInfo.State}&code={code}");
            }

            return BadRequest("Hmm...");
        }
    }
}
