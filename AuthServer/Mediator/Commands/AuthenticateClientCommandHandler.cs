using AuthServer.Domain.AggregatesModel.SessionAggregate;
using AuthServer.Service;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer.Mediator.Commands
{
    public class AuthenticateClientCommandHandler : ICommandHandler<AuthenticateClientCommand, SessionCommandResponce>
    {
        ISessionRepository SessionRepository;

        public AuthenticateClientCommandHandler(ISessionRepository sessionRepository)
        {
            SessionRepository = sessionRepository;
        }

        public async Task<SessionCommandResponce> Handle(AuthenticateClientCommand command)
        {
            Session session = SessionRepository.GetSessionByGrant(GrantType.code, command.AuthenticationCode);

            bool isSucceed = false;

            if (session != null && session.IsValid())
            {
                isSucceed = await SessionRepository.OpenSession(session.Id, command.AuthenticationCode);
            }

            return new SessionCommandResponce(isSucceed, session?.Id);
        }
    }
}
