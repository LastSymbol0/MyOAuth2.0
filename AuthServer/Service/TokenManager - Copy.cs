//using AuthServer.DAL;
//using AuthServer.Models;
//using AutoMapper;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Text;
//using System.Threading.Tasks;

//namespace AuthServer.Service
//{
//    public class TokenManager
//    {
//        private IMapper Mapper;

//        private IAuthorizedSessionsDAL AuthorizedSessions;

//        public TokenManager(IAuthorizedSessionsDAL authorizedSessions, IMapper mapper)
//        {
//            Mapper = mapper;
//            AuthorizedSessions = authorizedSessions;
//        }

//        public TokenResponceDTO GenerateTokenPair(IEnumerable<Claim> claims, string sessionId)
//        {
//            var tokenDTO = new TokenResponceDTO();

//            tokenDTO.AccessTokenExpirationDate = DateTime.UtcNow.AddMinutes(TokenLifetime);

//            JwtSecurityToken token = new JwtSecurityToken(
//                    issuer: TokenIssuer,
//                    audience: TokenAudience,
//                    notBefore: DateTime.UtcNow,
//                    claims: claims,
//                    expires: tokenDTO.AccessTokenExpirationDate,
//                    signingCredentials: new SigningCredentials(GetSymetricKey(), SecurityAlgorithms.HmacSha256)
//                );

//            tokenDTO.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);

//            tokenDTO.RefreshToken = GenerateRefreshToken(sessionId);

//            return tokenDTO;
//        }

//        public async Task<TokenResponceDTO> GenerateTokenPairAsync(string refreshToken)
//        {
//            var session = await AuthorizedSessions.GetSession(refreshToken);

//            if (session != null)
//            {
//                var tokenDTO = new TokenResponceDTO();

//                tokenDTO.AccessTokenExpirationDate = DateTime.UtcNow.AddMinutes(TokenLifetime);

//                var claims = Mapper.Map<IEnumerable<Claim>>(session.AccessParameters);

//                JwtSecurityToken token = new JwtSecurityToken(
//                        issuer: TokenIssuer,
//                        audience: TokenAudience,
//                        notBefore: DateTime.UtcNow,
//                        claims: claims,
//                        expires: tokenDTO.AccessTokenExpirationDate,
//                        signingCredentials: new SigningCredentials(GetSymetricKey(), SecurityAlgorithms.HmacSha256)
//                    );

//                tokenDTO.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);

//                tokenDTO.RefreshToken = GenerateRefreshToken(session.Id);

//                session.RefreshToken = tokenDTO.RefreshToken;
//                await AuthorizedSessions.PutSession(session);

//                return tokenDTO;
//            }
//            return null;
//        }

//        private string GenerateRefreshToken(string sessionId) => sessionId + Utils.Utils.GenerateRandomString(32);

//        public static SymmetricSecurityKey GetSymetricKey() => new SymmetricSecurityKey(Secret);

//        public static byte[] Secret = new HMACSHA256(Encoding.UTF8.GetBytes("MyVerySecretKeyString")).Key;
//        public const int TokenLifetime = 1;
//        public const string TokenIssuer = "MyAuthServer";
//        public const string TokenAudience = "MyAuthClient";

//        //private IDictionary<string, IEnumerable<Claim>> refreshTokens = new Dictionary<string, IEnumerable<Claim>>();
//    }
//}
