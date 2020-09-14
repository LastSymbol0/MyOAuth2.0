using AuthServer.Domain.AggregatesModel.SessionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Mediator.Queries
{
    public class GetClientAuthCodeQueryHandler : IQueryHandler<GetClientAuthCodeQuery, string>
    {
        ISessionRepository SessionsRepository;

        public GetClientAuthCodeQueryHandler(ISessionRepository sessionsRepository)
        {
            SessionsRepository = sessionsRepository;
        }

        public async Task<string> Handle(GetClientAuthCodeQuery query)
        {
            Session session = SessionsRepository.GetSessionById(query.SessionId);

            if (session != null && session.IsValid() && session.Status == SessionStatus.WaitingForClientAuthorization)
            {
                return session.ClientGrantValue;
            }

            return null;
        }
    }
}
