using FeedServiceSDK.Exceptions;
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
    public class SuccessEventArgs:EventArgs
    {
        public string Message { get; set; }
    }

    public class ErrorEventArgs : EventArgs
    {
        public IEnumerable<string> Errors { get; set; }
    }

    public class FeedServiceClient
    {
        public event EventHandler FeedServiceSuccessMessage;

        protected virtual void OnFeedServiceSuccessMessage(SuccessEventArgs e)
        {
            FeedServiceSuccessMessage?.Invoke(this, e);
        }

        public event EventHandler ExternalResourceErrors;

        protected virtual void OnExternalResourceErrors(ErrorEventArgs e)
        {
            ExternalResourceErrors?.Invoke(this, e);
        }

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
            var jObject = JObject.Parse(respContent);
            JToken value;
            if(jObject.TryGetValue("error", out value))
                throw new FeedServiceException(value.ToString());

            if (jObject.TryGetValue("success", out value))
            {
                OnFeedServiceSuccessMessage(new SuccessEventArgs { Message = value.ToString() });
               
                if (jObject.TryGetValue("result", out value))
                {
                    var result = value as JObject;
                    if (result.TryGetValue("access_token", out value))
                    {
                        _token = value.ToString();
                        return true;
                    }

                    return false;
                }
            }

            throw new FeedServiceException("Unknown error.");
        }

        public async Task<string> Register(string login, string password)
        {

            var content = JsonConvert.SerializeObject(new { Login = login, Password = password, Role = "user" });
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(SERVER_NAME + "/register"),
                Method = HttpMethod.Post,
                Content = new StringContent(content.ToString(), Encoding.UTF8, "application/json")
            };

            var response = await client.SendAsync(request);
            var respContent = await response.Content.ReadAsStringAsync();


            JObject jObject = JObject.Parse(respContent);
            JToken value;
            if (jObject.TryGetValue("success", out value))
            {
                OnFeedServiceSuccessMessage(new SuccessEventArgs { Message = value.ToString() });
                return value.ToString();
            }

            if (jObject.TryGetValue("error", out value))
                throw new FeedServiceException(value.ToString());

            

            throw new FeedServiceException("Unknown exception");
                
        }

        public async Task<int> CreateCollection(string name)
        {
            if (_token == null || _token == string.Empty)
                throw new UnAuthorizedException();

            var content = JsonConvert.SerializeObject(new { Name = name });
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(SERVER_NAME + "/Collections"),
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

            if (jObject.TryGetValue("success", out value))
            {
                OnFeedServiceSuccessMessage(new SuccessEventArgs { Message = value.ToString() });
                if (jObject.TryGetValue("result", out value))
                    return value.ToObject<Collection>().Id;
            }

            throw new FeedServiceException("Unknow error.");
            
        }

        public async Task<List<Collection>> GetCollections()
        {
            if (_token == null || _token == string.Empty)
                throw new UnAuthorizedException();

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(SERVER_NAME + $"/Collections"),
                Method = HttpMethod.Get
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await client.SendAsync(request);
            var respContent = await response.Content.ReadAsStringAsync();
            JObject jObject = JObject.Parse(respContent);

            if (jObject.TryGetValue("error", out JToken value))
                throw new FeedServiceException(value.ToString());

            if(jObject.TryGetValue("success", out value))
            {
                OnFeedServiceSuccessMessage(new SuccessEventArgs { Message = value.ToString() });
                if (jObject.TryGetValue("result", out value))
                    return value.ToObject<List<Collection>>();
            }

            throw new FeedServiceException("Unknown error");
        }

        public async Task<string> AddFeedToCollection(int collectionId, FeedType type, string url)
        {
            if (_token == null || _token == string.Empty)
                throw new UnAuthorizedException();

            var content = JsonConvert.SerializeObject(new { Url = url, Type = type });
            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(SERVER_NAME + $"/AddFeed/{collectionId}"),
                Method = HttpMethod.Put,
                Content = new StringContent(content.ToString(), Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);

            var response = await client.SendAsync(request);
            var respContent = await response.Content.ReadAsStringAsync();

            JObject jObject = JObject.Parse(respContent);
            JToken value;
            if (jObject.TryGetValue("error", out value))
                throw new FeedServiceException(value.ToString());

            if (jObject.TryGetValue("success", out value))
                return value.ToString();

            throw new FeedServiceException("Unknown error.");
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

            JObject jObject = JObject.Parse(respContent);
            if (jObject.TryGetValue("error", out JToken value))
                throw new FeedServiceException(value.ToString());
            if(jObject.TryGetValue("success", out value))
            {
                OnFeedServiceSuccessMessage(new SuccessEventArgs { Message = value.ToString() });
                if (jObject.TryGetValue("result", out value))
                {
                    var result = value as JObject;
                    if (result.TryGetValue("errors", out JToken errors))
                        OnExternalResourceErrors(new ErrorEventArgs { Errors = errors.ToObject<List<string>>() });
                    if (result.TryGetValue("news", out JToken news))
                        return news.ToObject<List<FeedItem>>();
                }
            }

            throw new FeedServiceException("Unknown error");
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
