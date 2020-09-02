using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthClient.Config
{
    public class ClientConfig
    {
        private IConfiguration Configuration { get; }
        public ClientConfig(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public string ClientId { get => Configuration.GetSection("ClientInfo").GetValue<string>("Id"); }
        public string ClientSecret { get => Configuration.GetSection("ClientInfo").GetValue<string>("Secret"); }

        public string AuthServerAuthEndpoint { get => Configuration.GetConnectionString("AuthServerAuthEndpoint"); }
        public string AuthServerTokenEndpoint { get => Configuration.GetConnectionString("AuthServerTokenEndpoint"); }
        public string ProtectedResourseURL { get => Configuration.GetConnectionString("ProtectedResourseURL"); }
        public string MyRedirectURL { get => Configuration.GetConnectionString("MyRedirectURL"); }

    }
}
