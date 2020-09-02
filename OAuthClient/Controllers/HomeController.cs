using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
            ViewBag.Res1 = await ResourcesHandler.GetResourcesAsync(1);
            ViewBag.Res2 = await ResourcesHandler.GetResourcesAsync(2);
            ViewBag.Res3 = await ResourcesHandler.GetResourcesAsync(3);
            ViewBag.Res4 = await ResourcesHandler.GetResourcesAsync(4);
            
            return View(TokenHandler.HasAccess());
        }

        [Route("ApplyCode/")]
        public IActionResult ApplyAuthCode([FromQuery(Name = "code")] string code)
        {
            return Redirect("~/Home");
        }

        public IActionResult Redirection()
        {
            return Redirect(@$"{Config.AuthServerAuthEndpoint}?redirectUrl={Config.MyRedirectURL}&clientId={Config.ClientId}&responceType=code");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
