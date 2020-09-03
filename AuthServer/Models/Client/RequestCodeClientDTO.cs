using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models
{
    public class RequestCodeClientDTO
    {
        public string ClientId { get; set;}
        public string ResponceType { get; set;}
        public string RedirectUrl { get; set;}
        public string State { get; set; }
    }
}
