using AuthServer.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Service
{
    public class ClientsAccessHandler
    {
        private class Client
        {
            public string clientId { get; set; }
            public AccessParameters accessParam { get; set; }
        }

        private Dictionary<string, Client> ClientsToAuth = new Dictionary<string, Client>();

        //private static string BoolToString(bool b) => b ? "1" : "0";

        public string GenerateAccessCode(RequestCodeClientDTO info, AccessParameters accessParam)
        {
            const string UserIdDummy = "UserIdDummy";
            string code =  $"{info.ClientId}{UserIdDummy}{new Random().Next()}";

            ClientsToAuth.Add(code, new Client { accessParam = accessParam, clientId = info.ClientId });

            return code;
        }

        public string GetClientToken(RequestTokenClientDTO clientDTO)
        {
            Client client;

            if (ClientsToAuth.TryGetValue(clientDTO.Code, out client))
            {
                IEnumerable<Claim> userClaims = client.accessParam.GetClaims();

                JwtSecurityToken token = new JwtSecurityToken(
                        issuer: TokenIssuer,
                        audience: TokenAudience,
                        notBefore: DateTime.UtcNow,
                        claims: userClaims,
                        expires: DateTime.UtcNow.AddMinutes(TokenLifetime),
                        signingCredentials: new SigningCredentials(GetSymetricKey(), SecurityAlgorithms.HmacSha256)
                    );

                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

                return handler.WriteToken(token);
            }

            return null;
        }

        public static SymmetricSecurityKey GetSymetricKey() => new SymmetricSecurityKey(Secret);

        public static byte[] Secret = new HMACSHA256(Encoding.UTF8.GetBytes("MyVerySecretKeyString")).Key;
        public const int TokenLifetime = 10;
        public const string TokenIssuer = "MyAuthServer";
        public const string TokenAudience = "MyAuthClient";
    }
}
