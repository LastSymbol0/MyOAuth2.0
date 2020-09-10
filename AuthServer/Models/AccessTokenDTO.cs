using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class AccessTokenDTO
    {
        public string AccessToken { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
