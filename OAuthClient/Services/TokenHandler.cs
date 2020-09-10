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
using OAuthClient.Models;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Threading;

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
                        await RequestToken(code);
                    }
                    else
                    {
                        throw new ArgumentException("Invalid state value recieved");
                    }
                }
            }

            // if page with protected resources
            if (context.Request.Path.Value.Equals("/Home"))
            {
                // if expired
                if (DateTime.Compare(AccessTokenExpirationDate, DateTime.UtcNow) < 0)
                {
                   bool isSucceed = await TryToRenewToken();

                   if (!isSucceed)
                   {
                        
                        // Redirect to auth with client page
                   }
                }
            }

            var identy = new ClaimsIdentity(TokenClaims);
            context.User.AddIdentity(identy);

            await next(context);
        }

        public async Task<bool> TryToRenewToken()
        {
            if (String.IsNullOrEmpty(RefreshToken))
            {
                return false;
            }

            AccessToken = null;

            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", RefreshToken)
            };

            var response = await HttpUtils.PostFormUrlEncoded(Сonfig.AuthServerTokenEndpoint, data, $"Basic {HttpUtils.GetBase64Creds(Сonfig.ClientId, Сonfig.ClientSecret)}");

            if (response.IsSuccessStatusCode)
            {
                string strResponce = await response.Content.ReadAsStringAsync();

                TokenDTO token = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenDTO>(strResponce);
                AccessToken = token.AccessToken;
                RefreshToken = token.RefreshToken;
                AccessTokenExpirationDate = token.AccessTokenExpirationDate;

                TokenClaims = HttpUtils.GetClaimsFromToken(AccessToken);

                return true;
            }
            else
            {
                // Unable to refresh token
                return false;
            }
        }

        private async Task RequestToken(string code)
        {
            List<KeyValuePair<string, string>> data = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("redirect_uri", Сonfig.MyRedirectURL),
            };

            var response = await HttpUtils.PostFormUrlEncoded(Сonfig.AuthServerTokenEndpoint, data, $"Basic {HttpUtils.GetBase64Creds(Сonfig.ClientId, Сonfig.ClientSecret)}");

            if (response.IsSuccessStatusCode)
            {
                string strResponce = await response.Content.ReadAsStringAsync();

                TokenDTO token = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenDTO>(strResponce);
                AccessToken = token.AccessToken;
                RefreshToken = token.RefreshToken;
                AccessTokenExpirationDate = token.AccessTokenExpirationDate;

                TokenClaims = HttpUtils.GetClaimsFromToken(AccessToken);
            }
            else
            {
                // Unable to refresh token
            }
        }

        public bool HasToken() => !String.IsNullOrEmpty(AccessToken) && DateTime.Compare(AccessTokenExpirationDate, DateTime.UtcNow) < 0;

        public string AccessToken { get; set; } = null;
        private string RefreshToken { get; set; } = null;
        private DateTime AccessTokenExpirationDate { get; set; }
        public IEnumerable<Claim> TokenClaims { get; set; } = null;

        public string State { get; set; } = null;
    }
}
