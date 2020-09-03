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
        private readonly ClientConfig Сonfig;

        public TokenHandler(ClientConfig clientConfig)
        {
            Сonfig = clientConfig;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.Request.Path.Value.Equals("/ApplyCode"))
            {
                if (context.Request.Query.TryGetValue("code", out StringValues code)
                    && context.Request.Query.TryGetValue("state", out StringValues state))
                {
                    if (state.Equals(State))
                    {
                        await TryToRenewToken(code);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid state value recieved");
                    }
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
                new KeyValuePair<string, string>("redirect_uri", Сonfig.MyRedirectURL),
            };

            Token = await HttpUtils.PostFormUrlEncoded(Сonfig.AuthServerTokenEndpoint, data, $"Basic {HttpUtils.GetBase64Creds(Сonfig.ClientId, Сonfig.ClientSecret)}");
        }

        public bool HasAccess() => !String.IsNullOrEmpty(Token);
        public string Token { get; set; } = null;
        public string State { get; set; } = null;
    }
}
