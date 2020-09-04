using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

namespace AuthServer.Models
{
    public class RequestTokenRefreshClientDTO
    {
        public string RefreshToken { get; set; }
    }
}