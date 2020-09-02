using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class AccessParameters
    {
        public bool Scope1Access { get; set; } = false;
        public bool Scope2Access { get; set; } = false;
        public bool Scope3Access { get; set; } = false;
        public bool Scope4Access { get; set; } = false;

        public IEnumerable<Claim> GetClaims()
        {
            return new List<Claim>
            {
                new Claim("Scope1Access", Scope1Access.ToString()),
                new Claim("Scope2Access", Scope2Access.ToString()),
                new Claim("Scope3Access", Scope3Access.ToString()),
                new Claim("Scope4Access", Scope4Access.ToString())
            };
        }
    }
}
