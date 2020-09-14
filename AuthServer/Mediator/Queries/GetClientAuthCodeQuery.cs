using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Mediator.Queries
{
    public class GetClientAuthCodeQuery : IQuery<string>
    {
        public int SessionId { get; set; }
    }
}
