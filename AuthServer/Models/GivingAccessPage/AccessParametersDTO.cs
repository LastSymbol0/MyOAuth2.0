using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class AccessParametersDTO
    {
        public bool Scope1Access { get; set; } = false;
        public bool Scope2Access { get; set; } = false;
        public bool Scope3Access { get; set; } = false;
        public bool Scope4Access { get; set; } = false;
    }
}
