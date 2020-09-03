using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OAuthClient.Models
{
    public class TokenDTO
    {
        public string AccessToken { get; set; } = null;
        public string RefreshToken { get; set; } = null;
        public DateTime AccessTokenExpirationDate { get; set; }
    }
}
