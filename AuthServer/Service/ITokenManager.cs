using AuthServer.Models;
using System.Collections.Generic;
using System.Security.Claims;

namespace AuthServer.Service
{
    public interface ITokenManager
    {
        public AccessTokenDTO GenerateAccessToken(IEnumerable<Claim> claims);
    }
}
