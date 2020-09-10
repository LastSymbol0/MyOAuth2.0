using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Mediator
{
    public class SessionCommandResponce
    {
        public SessionCommandResponce(bool isSucceed, int? sessionId)
        {
            IsSucceed = isSucceed;
            SessionId = sessionId;
        }

        public bool IsSucceed { get; set; }
        public int? SessionId { get; set; }
    }
}
