using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ResourceServer.Authentication
{
    public class AuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        readonly Uri AuthenticationServiceUri;
        public AuthenticationHandler(
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IConfiguration configuration)
            : base(options, logger, encoder, clock)
        {
            AuthenticationServiceUri = new Uri(configuration.GetConnectionString("AuthServerAuthEndpoint"));
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
            {
                return AuthenticateResult.Fail("Missing Authorization Header");
            }

            string authVal = Request.Headers.SingleOrDefault(header => header.Key == "Authorization").Value;

            HttpRequestMessage authenticationRequest = new HttpRequestMessage();

            authenticationRequest.RequestUri = AuthenticationServiceUri;
            authenticationRequest.Method = HttpMethod.Get;
            authenticationRequest.Headers.TryAddWithoutValidation("Authorization", authVal);

            var result = await new HttpClient().SendAsync(authenticationRequest);

            string strResponse = await result.Content.ReadAsStringAsync();

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return AuthenticateResult.Fail($"Request error.\n{strResponse}");
            }

            var claims = GetClaimsFromToken(authVal.Substring("Bearer ".Length).Trim()); 
            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);

            return AuthenticateResult.Success(ticket);
        }

        private IEnumerable<Claim> GetClaimsFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var DecodedToken = handler.ReadJwtToken(token);

            return DecodedToken.Claims;
        }

    }
}
