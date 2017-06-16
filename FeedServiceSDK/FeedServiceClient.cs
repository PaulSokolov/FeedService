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
            var content = new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("username", login),
                    new KeyValuePair<string, string>("password", password)
                });
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

            var response = await client.SendAsync(request);
            var respContent = await response.Content.ReadAsStringAsync();


            JObject jObject = JObject.Parse(respContent);
            JToken value;
            if (jObject.TryGetValue("error", out value))
                throw new CreateCollectionException(value.ToString());

            var id = jObject["id"].ToString();
            return int.Parse(id);
        }

        public async Task<List<Collection>> GetCollections()
        {
            if (_token == null || _token == string.Empty)
                throw new UnAuthorizedException();

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(SERVER_NAME + $"/api/Collections"),
                Method = HttpMethod.Get
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await client.SendAsync(request);
            var respContent = await response.Content.ReadAsStringAsync();
            var col = JsonConvert.DeserializeObject<List<Collection>>(respContent);

            return col;
        }

        public async Task<string> AddFeedToCollection(int collectionId, FeedType type, string url)
        {
            if (_token == null || _token == string.Empty)
                throw new UnAuthorizedException();

            var content = JsonConvert.SerializeObject(new { Url = url, Type = type });
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(SERVER_NAME + $"/AddToCollection/{collectionId}"),
                Method = HttpMethod.Put,
                Content = new StringContent(content.ToString(), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await client.SendAsync(request);
            var respContent = await response.Content.ReadAsStringAsync();

            JObject jObject = JObject.Parse(respContent);
            var answer = jObject.ToString();
            return answer;
        }

        public async Task<List<FeedItem>> ReadNewsFromCollection(int collectionId)
        {
            if (_token == null || _token == string.Empty)
                throw new UnAuthorizedException();

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(SERVER_NAME + $"/GetNews/{collectionId}"),
                Method = HttpMethod.Get
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await client.SendAsync(request);
            var respContent = await response.Content.ReadAsStringAsync();
            var news = JsonConvert.DeserializeObject<List<FeedItem>>(respContent);

            return news;
        }
    }

    public class Collection
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return $"Id: {Id} - {Name}";
        }
    }

    public class FeedItem
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string Link { get; set; }
        public DateTime PublishedDate { get; set; }

        public override string ToString()
        {
            return $"Title: {Title} - published {PublishedDate.ToShortDateString()}\nLink: {Link}\nContent: {Content}";
        }
    }

    public enum FeedType
    {
        RSS,
        RDF,
        Atom
    }
}
