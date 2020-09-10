using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Mediator.Queries
{
    public class GetTokenPairQuery : IQuery<GetTokenPairQueryDTO>
    {
        public int SessionId { get; set; }
    }
}
