using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using OAuthClient.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using OAuthClient.Utils;

namespace OAuthClient.Services
{
    public class TokenHandler : IMiddleware
    {
        private readonly ILogger<TokenHandler> _logger;

        private readonly ClientConfig config;

        public TokenHandler(ILogger<TokenHandler> logger, ClientConfig clientConfig)
        {
            _logger = logger;
            config = clientConfig;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (!HasAccess())
            {
                StringValues code;
                if (context.Request.Query.TryGetValue("code", out code))
                {
                    await TryToRenewToken(code);
                }
            }

            await next(context);
        }

        private async Task TryToRenewToken(string code)
        {
            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", config.MyRedirectURL),
            };

            Token = await HttpUtils.PostFormUrlEncoded(config.AuthServerTokenEndpoint, data, $"Basic {HttpUtils.GetBase64Creds(config.ClientId, config.ClientSecret)}");
        }

        public bool HasAccess() => !String.IsNullOrEmpty(Token);
        public string Token { get; set; } = null;
    }
}
