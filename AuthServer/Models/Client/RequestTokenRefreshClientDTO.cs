using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

namespace AuthServer.Models
{
    public class RequestTokenRefreshClientDTO
    {
        public RequestTokenRefreshClientDTO() { }
        public RequestTokenRefreshClientDTO(IFormCollection form) => FromForm(form);

        public string RefreshToken { get; set; }

        public void FromForm(IFormCollection form)
        {
            if (form.TryGetValue("refresh_token", out StringValues refreshToken))
            {
                RefreshToken = refreshToken;
            }
        }
    }
}