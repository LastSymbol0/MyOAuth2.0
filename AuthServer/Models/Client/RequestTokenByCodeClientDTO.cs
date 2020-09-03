using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;

namespace AuthServer.Models
{
    public class RequestTokenByCodeClientDTO
    {
        public RequestTokenByCodeClientDTO() { }
        public RequestTokenByCodeClientDTO(IFormCollection form) => FromForm(form);

        public string Code { get; set; }
        public string RedirectUrl { get; set; }

        public void FromForm(IFormCollection form)
        {
            if (form.TryGetValue("code", out StringValues code))
            {
                Code = code;
            }

            if (form.TryGetValue("redirect_uri", out StringValues redirectUrl))
            {
                RedirectUrl = redirectUrl;
            }
        }
    }
}