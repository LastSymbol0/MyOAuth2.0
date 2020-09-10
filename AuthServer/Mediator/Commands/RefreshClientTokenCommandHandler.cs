using AuthServer.Domain.AggregatesModel.SessionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Mediator.Commands
{
    public class RefreshClientTokenCommandHandler : ICommandHandler<RefreshClientTokenCommand, SessionCommandResponce>
    {
        ISessionRepository SessionRepository;

        public RefreshClientTokenCommandHandler(ISessionRepository sessionRepository)
        {
            SessionRepository = sessionRepository;
        }

        public async Task<SessionCommandResponce> Handle(RefreshClientTokenCommand command)
        {
            Session session = SessionRepository.GetSessionByGrant(GrantType.refresh_token, command.RefreshToken);

            bool isSucceed = false;

            if (session != null && session.IsValid() && session.IsOpen()
                && session.ClientId == int.Parse(command.ClientId))
            {
                isSucceed = await SessionRepository.RefreshToken(session.Id, command.RefreshToken);
            }

            return new SessionCommandResponce(isSucceed, session?.Id);
        }
    }
}
