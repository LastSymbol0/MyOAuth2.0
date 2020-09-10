using System;
using System.Collections.Generic;
using System.Text;

namespace AuthServer.Domain.AggregatesModel.SessionAggregate
{
    public enum GrantType
    {
        None,
        code,
        refresh_token
    }
}
