using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AuthServer.Domain
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

        public static int GenerateRandomInt() => RandomNumberGenerator.GetInt32(int.MaxValue);
    }
}
