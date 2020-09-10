using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer.Models.GivingAccessPage
{
    public class GivingAccessModel
    {
        public AccessParametersDTO  AccessParameters { get; set;}
        public RequestCodeClientDTO  ClientInfo {get; set;}
    }
}
