using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace AuthServer.Models
{
    public class RequestTokenClientDTO
    {
        public string Code { get; set; }
        public string GrantType { get; set; }
        public string RedirectUrl { get; set; }

        public void FromForm(IFormCollection form)
        {
            if (form.TryGetValue("code", out StringValues code))
            {
                Code = code;
            }

            if (form.TryGetValue("grant_type", out StringValues grantType))
            {
                GrantType = grantType;
            }

            if (form.TryGetValue("redirect_uri", out StringValues redirectUrl))
            {
                RedirectUrl = redirectUrl;
            }
        }
    }
}