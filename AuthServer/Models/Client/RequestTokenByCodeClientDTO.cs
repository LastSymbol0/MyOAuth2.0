using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

namespace AuthServer.Models
{
    public class RequestTokenByCodeClientDTO
    {
        public string Code { get; set; }
        public string RedirectUrl { get; set; }
    }
}