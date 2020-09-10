using AuthServer.Models;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service
{
    public class TokenManager
    {
        public AccessTokenDTO GenerateAccessToken(IEnumerable<Claim> claims)
        {
            var tokenDTO = new AccessTokenDTO();

            tokenDTO.ExpirationDate = DateTime.UtcNow.AddMinutes(TokenLifetime);

            JwtSecurityToken token = new JwtSecurityToken(
                    issuer: TokenIssuer,
                    audience: TokenAudience,
                    notBefore: DateTime.UtcNow,
                    claims: claims,
                    expires: tokenDTO.ExpirationDate,
                    signingCredentials: new SigningCredentials(GetSymetricKey(), SecurityAlgorithms.HmacSha256)
                );

            tokenDTO.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return tokenDTO;
        }

        public static SymmetricSecurityKey GetSymetricKey() => new SymmetricSecurityKey(Secret);

        public static byte[] Secret = new HMACSHA256(Encoding.UTF8.GetBytes("MyVerySecretKeyString")).Key;
        public const int TokenLifetime = 1;
        public const string TokenIssuer = "MyAuthServer";
        public const string TokenAudience = "MyAuthClient";
    }
}
