using AuthServer.Domain.AggregatesModel.SessionAggregate;
using AuthServer.Service;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer.Mediator.Queries
{
    public class GetTokenPairQueryHandler : IQueryHandler<GetTokenPairQuery, GetTokenPairQueryDTO>
    {
        ISessionRepository SessionRepository;
        TokenManager TokenManager;
        private IMapper Mapper;

        public GetTokenPairQueryHandler(
            ISessionRepository sessionRepository,
            TokenManager tokenManager,
            IMapper mapper)
        {
            SessionRepository = sessionRepository;
            TokenManager = tokenManager;
            Mapper = mapper;
        }

        public async Task<GetTokenPairQueryDTO> Handle(GetTokenPairQuery query)
        {
            Session session = SessionRepository.GetSessionById(query.SessionId);

            if (session != null && session.IsValid() && session.IsOpen())
            {
                GetTokenPairQueryDTO responce = new GetTokenPairQueryDTO();

                var claims = Mapper.Map<IEnumerable<Claim>>(session.AccessParameters);

                var accessToken = TokenManager.GenerateAccessToken(claims);

                responce.AccessToken = accessToken.AccessToken;
                responce.AccessTokenExpirationDate = accessToken.ExpirationDate;
                responce.RefreshToken = session.ClientGrantValue;

                return responce;
            }

            return null;
        }
    }
}
