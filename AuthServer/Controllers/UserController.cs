using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Models;
using AuthServer.Models.GivingAccessPage;
using AuthServer.Service;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    public class UserController : Controller
    {
        private readonly ClientsAccessHandler AccessHandler;
        public UserController(ClientsAccessHandler accessHandler)
        {
            AccessHandler = accessHandler;
        }

        [HttpGet]
        public IActionResult GetAccessToClient([FromQuery(Name = "clientId")] string clientId,
                                                [FromQuery(Name = "responceType")] string responceType,
                                                [FromQuery(Name = "redirectUrl")] string redirectUrl,
                                                [FromQuery(Name = "state")] string state)
        {
            RequestCodeClientDTO info = new RequestCodeClientDTO();

            info.ClientId = clientId;
            info.ResponceType = responceType;
            info.RedirectUrl = redirectUrl;
            info.State = state;

            return View(new GivingAccessModel{ ClientInfo = info, AccessParameters = new AccessParameters() });
        }

        [HttpPost]
        public IActionResult GetAccessToClient(GivingAccessModel model)
        {
            return Redirect($"{model.ClientInfo.RedirectUrl}?state={model.ClientInfo.State}&code={AccessHandler.GenerateAccessCode(model.ClientInfo, model.AccessParameters)}");
        }
    }
}
