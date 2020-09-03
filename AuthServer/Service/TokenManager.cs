using AuthServer.Models;
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
        public TokenResponceDTO GenerateTokenPair(IEnumerable<Claim> claims)
        {
            var tokenDTO = new TokenResponceDTO();

            tokenDTO.AccessTokenExpirationDate = DateTime.UtcNow.AddMinutes(TokenLifetime);

            JwtSecurityToken token = new JwtSecurityToken(
                    issuer: TokenIssuer,
                    audience: TokenAudience,
                    notBefore: DateTime.UtcNow,
                    claims: claims,
                    expires: tokenDTO.AccessTokenExpirationDate,
                    signingCredentials: new SigningCredentials(GetSymetricKey(), SecurityAlgorithms.HmacSha256)
                );

            tokenDTO.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);

            tokenDTO.RefreshToken = GenerateRefreshToken(claims);

            return tokenDTO;
        }

        public TokenResponceDTO GenerateTokenPair(string refreshToken)
        {
            if (refreshTokens.TryGetValue(refreshToken, out IEnumerable<Claim> claims))
            {
                refreshTokens.Remove(refreshToken);

                var tokenDTO = new TokenResponceDTO();

                tokenDTO.AccessTokenExpirationDate = DateTime.UtcNow.AddMinutes(TokenLifetime);

                JwtSecurityToken token = new JwtSecurityToken(
                        issuer: TokenIssuer,
                        audience: TokenAudience,
                        notBefore: DateTime.UtcNow,
                        claims: claims,
                        expires: tokenDTO.AccessTokenExpirationDate,
                        signingCredentials: new SigningCredentials(GetSymetricKey(), SecurityAlgorithms.HmacSha256)
                    );

                tokenDTO.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);

                tokenDTO.RefreshToken = GenerateRefreshToken(claims);

                return tokenDTO;
            }
            return null;
        }

        private string GenerateRefreshToken(IEnumerable<Claim> claims)
        {
            var randomNumber = new byte[32];
            using (var r = RandomNumberGenerator.Create())
            {
                r.GetBytes(randomNumber);
                string refreshToken = Convert.ToBase64String(randomNumber);

                refreshTokens.Add(refreshToken, claims);

                return refreshToken;
            }
        }

        public static SymmetricSecurityKey GetSymetricKey() => new SymmetricSecurityKey(Secret);

        public static byte[] Secret = new HMACSHA256(Encoding.UTF8.GetBytes("MyVerySecretKeyString")).Key;
        public const int TokenLifetime = 1;
        public const string TokenIssuer = "MyAuthServer";
        public const string TokenAudience = "MyAuthClient";

        private IDictionary<string, IEnumerable<Claim>> refreshTokens = new Dictionary<string, IEnumerable<Claim>>();
    }
}
