using System;
using System.Collections.Generic;
using System.Text;

namespace AuthServer.Domain.AggregatesModel.SessionAggregate
{
    public enum SessionStatus
    {
        None,
        WaitingForClientAuthorization,
        Open,
        Closed
    }
}
