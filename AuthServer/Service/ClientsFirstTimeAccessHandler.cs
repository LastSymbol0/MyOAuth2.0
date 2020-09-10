//using AuthServer.DAL;
//using AuthServer.Models;
//using AuthServer.Utils;
//using AutoMapper;
//using Microsoft.IdentityModel.Tokens;
//using System;
//using System.Collections.Generic;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;
//using System.Security.Claims;
//using System.Security.Cryptography;
//using System.Security.Cryptography.X509Certificates;
//using System.Text;
//using System.Threading.Tasks;

//namespace AuthServer.Service
//{
//    public class ClientsFirstTimeAccessHandler
//    {
//        private IInitSessionsDAL InitSessionsDAL;

//        private TokenManager TokenManager;

//        private IMapper Mapper;

//        public ClientsFirstTimeAccessHandler(TokenManager tokenManager,
//            IMapper mapper,
//            IInitSessionsDAL initSessionsDAL)
//        {
//            TokenManager = tokenManager;
//            Mapper = mapper;
//            InitSessionsDAL = initSessionsDAL;
//        }

//        public string GenerateAccessCode(RequestCodeClientDTO info, AccessParameters accessParam)
//        {
//            const string UserIdDummy = "UserIdDummy";

//            InitSession initSesssion = new InitSession();

//            initSesssion.Id = $"{info.ClientId}{UserIdDummy}";
//            initSesssion.Code = $"{initSesssion.Id}{Utils.Utils.GenerateRandomString(16)}";
//            initSesssion.ExpireIn = DateTime.UtcNow.AddSeconds(30.0);
//            initSesssion.AccessParameters = accessParam;

//            InitSessionsDAL.PutSession(initSesssion);

//            return initSesssion.Code;
//        }

//        public async Task<AccessTokenDTO> GetClientTokenAsync(RequestTokenByCodeClientDTO requestDTO)
//        {
//            InitSession session = await InitSessionsDAL.GetSession(requestDTO.Code);

//            if (session != null)
//            {
//                await InitSessionsDAL.DeleteSession(session.Code);

//                var claims = Mapper.Map<IEnumerable<Claim>>(session.AccessParameters);

//                return TokenManager.GenerateTokenPair(claims, session.Id);
//            }

//            return null;
//        }
//    }
//}
