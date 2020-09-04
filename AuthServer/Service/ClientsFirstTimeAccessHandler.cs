using AuthServer.Models;
using AutoMapper;
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
    public class ClientsFirstTimeAccessHandler
    {
        private class Client
        {
            public string clientId { get; set; } // authorization_code
            public AccessParameters accessParam { get; set; }
        }

        private Dictionary<string, Client> ClientsToAuth = new Dictionary<string, Client>();

        private TokenManager TokenManager;

        private IMapper Mapper;

        public ClientsFirstTimeAccessHandler(TokenManager tokenManager, IMapper mapper)
        {
            TokenManager = tokenManager;
            Mapper = mapper;
        }

        public string GenerateAccessCode(RequestCodeClientDTO info, AccessParameters accessParam)
        {
            const string UserIdDummy = "UserIdDummy";
            string code =  $"{info.ClientId}{UserIdDummy}{new Random().Next()}";

            ClientsToAuth.Add(code, new Client { accessParam = accessParam, clientId = info.ClientId });

            return code;
        }

        public TokenResponceDTO GetClientToken(RequestTokenByCodeClientDTO clientDTO)
        {
            if (ClientsToAuth.TryGetValue(clientDTO.Code, out Client client))
            {
                ClientsToAuth.Remove(clientDTO.Code);

                var claims = Mapper.Map<IEnumerable<Claim>>(client.accessParam);

                return TokenManager.GenerateTokenPair(claims);
            }

            return null;
        }


    }
}
