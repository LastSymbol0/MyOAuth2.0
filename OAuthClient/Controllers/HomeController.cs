using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OAuthClient.Config;
using OAuthClient.Models;
using OAuthClient.Services;

namespace OAuthClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ClientConfig Config;

        private readonly TokenHandler TokenHandler;
        private readonly ResourcesHandler ResourcesHandler;

        public HomeController(ClientConfig clientConfig,
            TokenHandler tokenHandler,
            ResourcesHandler resourcesHandler)
        {
            Config = clientConfig;
            TokenHandler = tokenHandler;
            ResourcesHandler = resourcesHandler;
        }

        public async Task<IActionResult> IndexAsync()
        {
            if (User.HasClaim(claim => claim.Type == "Scope1Access" && claim.Value == "True"))
            {
                ViewBag.Res1 = await ResourcesHandler.GetResourcesAsync(1);
            }

            if (User.HasClaim(claim => claim.Type == "Scope2Access" && claim.Value == "True"))
            {
                ViewBag.Res2 = await ResourcesHandler.GetResourcesAsync(2);
            }

            if (User.HasClaim(claim => claim.Type == "Scope3Access" && claim.Value == "True"))
            {
                ViewBag.Res3 = await ResourcesHandler.GetResourcesAsync(3);
            }

            if (User.HasClaim(claim => claim.Type == "Scope4Access" && claim.Value == "True"))
            {
                ViewBag.Res4 = await ResourcesHandler.GetResourcesAsync(4);
            }

            return View(TokenHandler.HasToken());
        }

        [Route("ApplyCode/")]
        public IActionResult ApplyAuthCode([FromQuery(Name = "code")] string code)
        {
            return Redirect("~/Home");
        }

        public IActionResult Redirection()
        {
            string state = new Random().Next().ToString();

            TokenHandler.State = state;

            return Redirect(@$"{Config.AuthServerAuthEndpoint}?redirectUrl={Config.MyRedirectURL}&clientId={Config.ClientId}&responceType=code&state={state}");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
