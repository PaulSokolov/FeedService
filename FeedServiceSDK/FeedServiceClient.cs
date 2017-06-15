using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FeedServiceSDK
{
    public class FeedServiceClient
    {
        private const string SERVER_NAME = "http://localhost:60840";
        private HttpClient client = new HttpClient();
        private string _token;

        public async Task<bool> Authorize(string login, string password)
        {
            string requestUri = SERVER_NAME + "/token";
            var content = new StringContent(JsonConvert.SerializeObject(
                new { username = login, password = password })
                .ToString(),
                Encoding.UTF8, "application/json");

            var response = await client.PostAsync(requestUri, content);
            var respContent = await response.Content.ReadAsStringAsync();
            if (respContent.Contains("Invalid username or password."))
                return false;
            if (respContent.Contains("access_token"))
            {
                JObject jObject = JObject.Parse(respContent);
                _token = jObject["access_token"].ToString();
                return true;
            }
            return false;
        }

        public async Task<int> CreateCollection(string name)
        {
            if (_token == null || _token == string.Empty)
                throw new UnAuthorizedException();

            var content = JsonConvert.SerializeObject(new { Name = name });
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(SERVER_NAME + "api/Collections"),
                Method = HttpMethod.Post,
                Content = new StringContent(content.ToString(), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue($"Authorization : Bearer {_token}"));
            var response = await client.SendAsync(request);

            JObject jObject = JObject.Parse(response.Content.ToString());
            var id = jObject["id"].ToString();
            return int.Parse(id);
        }
    }
}
