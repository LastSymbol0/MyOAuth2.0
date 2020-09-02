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
        private readonly ILogger<HomeController> _logger;

        private readonly ClientConfig config;

        private readonly TokenHandler tokenHandler;
        private readonly ResourcesHandler resourcesHandler;

        public HomeController(ILogger<HomeController> logger,
            ClientConfig clientConfig,
            TokenHandler tokenHandler,
            ResourcesHandler resourcesHandler)
        {
            _logger = logger;
            config = clientConfig;
            this.tokenHandler = tokenHandler;
            this.resourcesHandler = resourcesHandler;
        }

        public async Task<IActionResult> IndexAsync()
        {
            ViewBag.Res1 = await resourcesHandler.GetResourcesAsync(1);
            ViewBag.Res2 = await resourcesHandler.GetResourcesAsync(2);
            ViewBag.Res3 = await resourcesHandler.GetResourcesAsync(3);
            ViewBag.Res4 = await resourcesHandler.GetResourcesAsync(4);
            
            return View(tokenHandler.HasAccess());
        }

        [Route("ApplyCode/")]
        public IActionResult ApplyAuthCode([FromQuery(Name = "code")] string code)
        {
            //Here must be request to auth server to get token from code
            return Redirect("~/Home");
        }

        public IActionResult Redirection()
        {
            return Redirect(@$"{config.AuthServerAuthEndpoint}?redirectUrl={config.MyRedirectURL}&clientId={config.ClientId}&responceType=code");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
