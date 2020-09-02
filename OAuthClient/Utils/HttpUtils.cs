using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace OAuthClient.Utils
{
    public static class HttpUtils
    {
        public static string GetBase64Creds(string login, string password)
        {
            string creds = $"{login}:{password}";
            var plainTextBytes = Encoding.UTF8.GetBytes(creds);

            return Convert.ToBase64String(plainTextBytes);
        }

        public static async Task<string> PostFormUrlEncoded(string url, IEnumerable<KeyValuePair<string, string>> postData, string authValue = null)
        {
            using (var httpClient = new HttpClient())
            {
                using (var content = new FormUrlEncodedContent(postData))
                {
                    content.Headers.Clear();
                    content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");

                    HttpRequestMessage requestMessage = new HttpRequestMessage();
                    requestMessage.RequestUri = new Uri(url);
                    requestMessage.Content = content;
                    requestMessage.Method = HttpMethod.Post;

                    if (!String.IsNullOrEmpty(authValue))
                    {
                        requestMessage.Headers.TryAddWithoutValidation("Authorization", authValue);
                    }

                    HttpResponseMessage response = await httpClient.SendAsync(requestMessage);

                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}
