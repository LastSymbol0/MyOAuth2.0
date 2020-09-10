using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Mediator.Commands
{
    public class AuthenticateClientCommand : ICommand<SessionCommandResponce>
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }

        public string AuthenticationCode { get; set; }
    }
}
