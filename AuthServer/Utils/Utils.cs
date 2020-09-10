using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Utils
{
    public class Utils
    {
        public static string GenerateRandomString(long lenght)
        {
            var randomNumber = new byte[lenght];

            using (var r = RandomNumberGenerator.Create())
            {
                r.GetBytes(randomNumber);
                string randomString = Convert.ToBase64String(randomNumber);

                return randomString;
            }
        }

        public static (string, string) GetLoginPasswordFromAuthHeader(string authHeaderValue)
        {
            string encodedUsernamePassword = authHeaderValue.Substring("Basic ".Length).Trim();

            var encoding = Encoding.UTF8;
            string usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

            int separatorIndex = usernamePassword.IndexOf(':');
             string login = usernamePassword.Substring(0, separatorIndex);
            string password = usernamePassword.Substring(separatorIndex + 1);

            return (login, password);
        }
    }
}
