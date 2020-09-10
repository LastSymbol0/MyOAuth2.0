using AuthServer.Domain.AggregatesModel.SessionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Mediator.Commands
{
    public class StartSessionCommandHandler : ICommandHandler<StartSessionCommand, SessionCommandResponce>
    {
        ISessionRepository SessionRepository;

        public StartSessionCommandHandler(ISessionRepository sessionRepository)
        {
            SessionRepository = sessionRepository;
        }

        public async Task<SessionCommandResponce> Handle(StartSessionCommand command)
        {
            try
            {
                // For now creating client and RO here is redundant
                // Ideally, needs to be added contexts/repos for clients and ROs
                Client client = new Client(int.Parse(command.ClientId), command.ClientSecret);
                ResourceOwner resourceOwner = new ResourceOwner(int.Parse(command.ResourceOwnerId));

                Session session = new Session(client.Id, resourceOwner.Id, command.AccessParameters);

                if (SessionRepository.GetSessionById(session.Id) != null)
                {
                    return new SessionCommandResponce(false, null);
                }

                await SessionRepository.AddSession(session);

                return new SessionCommandResponce(true, session?.Id);
            }
            catch (Exception)
            {
                return new SessionCommandResponce(false, null);
            }
        }
    }
}
