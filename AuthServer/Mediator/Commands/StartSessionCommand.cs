using AuthServer.Domain.AggregatesModel.SessionAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Mediator.Commands
{
    public class StartSessionCommand : ICommand<SessionCommandResponce>
    {
        public string ResourceOwnerId { get; set; }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public AccessParameters AccessParameters { get; set; }
    }
}
