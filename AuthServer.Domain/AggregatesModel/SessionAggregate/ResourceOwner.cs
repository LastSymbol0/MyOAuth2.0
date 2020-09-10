using AuthServer.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthServer.Domain.AggregatesModel.SessionAggregate
{
    public class ResourceOwner : Entity
    {
        public ResourceOwner(int id)
        {
            Id = id;
        }
    }
}
