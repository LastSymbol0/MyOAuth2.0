using OAuthClient.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace OAuthClient.Services
{
    public class ResourcesHandler
    {
        private readonly ClientConfig config;

        private readonly TokenHandler tokenHandler;

        public ResourcesHandler(ClientConfig clientConfig, TokenHandler tokenHandler)
        {
            config = clientConfig;
            this.tokenHandler = tokenHandler;
        }

        public async Task<string> GetResourcesAsync(int number)
        {
            string url = config.ProtectedResourseURL;

            switch (number)
            {
                case 1:
                    {
                        url += "Get1";
                        break;
                    }
                case 2:
                    {
                        url += "Get2";
                        break;
                    }
                case 3:
                    {
                        url += "Get3";
                        break;
                    }
                case 4:
                    {
                        url += "Get4";
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }

            HttpRequestMessage request = new HttpRequestMessage();

            request.RequestUri = new Uri(url);
            request.Method = HttpMethod.Get;
            request.Headers.TryAddWithoutValidation("Authorization", $"Bearer {tokenHandler.Token}");

            var result = await new HttpClient().SendAsync(request);

            string strResponse = await result.Content.ReadAsStringAsync();

            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return null;
            }

            return strResponse;
        }

    }
}
