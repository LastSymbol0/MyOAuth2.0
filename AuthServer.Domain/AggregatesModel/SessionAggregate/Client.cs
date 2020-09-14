using AuthServer.Domain.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthServer.Domain.AggregatesModel.SessionAggregate
{
    public class Client : Entity
    {
        public string ClientSecret { get; set; }

        public Client() { }
        public Client(int clientId, string clientSecret)
        {
            Id = clientId;
            ClientSecret = clientSecret ?? throw new ArgumentNullException();
        }
    }
}
