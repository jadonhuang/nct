using Nct.Extentions;
using Nct.Ioc;
using Nct.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Nct.Http.ContentRequest
{
    public class ContentRequest : IContentRequest, ITransientDependency
    {

        public const string ClientName = "CommonClient";
        private readonly IHttpClientFactory _httpClientFactory;
        public ContentRequest(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;

        }

        public async Task<HttpContent> RequestAsync(string url, HttpMethod method, HttpContent content,
            IDictionary<string, string> headers = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {


            var requestMessage = new HttpRequestMessage(method, url);

            if (content != null)
            {

                requestMessage.Content = content;

            }

            if (headers != null)
            {
                foreach (var kv in headers)
                {
                    requestMessage.Headers.Add(kv.Key, kv.Value);

                }
            }

            var client = _httpClientFactory.CreateClient(ClientName);
            if (timeout.HasValue)
            {
                client.Timeout = timeout.Value;
            }

            var responseMessage = await client.SendAsync(requestMessage, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            if (responseMessage.StatusCode == System.Net.HttpStatusCode.OK)
                return responseMessage.Content;

            throw new Exception($"HttpStatusCode is {responseMessage.StatusCode}");


        }

        public async Task<T> GetAsync<T>(string url,
           IDictionary<string, string> headers = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {

            var responseContent = await RequestAsync(url, HttpMethod.Get, null, headers, timeout, cancellationToken);
            return await HandleResultAsync<T>(responseContent);

        }

        public async Task<T> PostAsync<T>(string url, object data, bool isFormData = false,
         IDictionary<string, string> headers = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
        {

            var content = BuildRequestContent(data, isFormData);
            var responseContent = await RequestAsync(url, HttpMethod.Post, content, headers, timeout, cancellationToken);
            return await HandleResultAsync<T>(responseContent);


        }

        private static async Task<T> HandleResultAsync<T>(HttpContent responseContent)
        {

            var resultType = typeof(T);
            if (resultType == typeof(Stream))
            {
                var stream = await responseContent.ReadAsStreamAsync();
                return (T)(object)stream;

            }

            var stringContent = await responseContent.ReadAsStringAsync();
            if (typeof(T) == typeof(string))
            {
                return (T)(object)stringContent;
            }

            if (stringContent.IsEmpty())
            {
                return default;
            }

            return stringContent.ToObject<T>();

        }

        private static HttpContent BuildRequestContent(object data, bool isFormData)
        {
            if (data == null) return null;
            HttpContent content;
            if (!isFormData)
            {
                content = new StringContent(data.ToJson(), Encoding.UTF8, MimeTypes.Json);
            }
            else
            {
                var kvList = ObjectToKeyValue(data);
                content = new FormUrlEncodedContent(kvList);
                content.Headers.ContentType = MediaTypeHeaderValue.Parse(MimeTypes.XWwwFormUrlencoded);
            }
            return content;
        }

        private static string NodePathToKey(string path)
        {
            var paths = path.Split('.').Skip(1).ToList();
            if (paths.Count > 1)
            {
                return paths[0] + string.Join("", paths.Skip(1).Select(m => $"[{m}]"));
            }

            return paths[0];

        }

        public static List<KeyValuePair<string, string>> ObjectToKeyValue(object obj)
        {

            var data = new List<KeyValuePair<string, string>>();
            if (obj == null) return data;

            var node = obj as JsonNode;
            node ??= JsonSerializer.SerializeToNode(obj);

            var valueKind = node.GetValueKind();

            if (valueKind == JsonValueKind.Object)
            {
                var jObj = node as JsonObject;
                foreach (var n in jObj)
                {
                    var childData = ObjectToKeyValue(n.Value);
                    data.AddRange(childData);
                }

            }
            else if (valueKind == JsonValueKind.Array)
            {
                var array = node as JsonArray;
                foreach (var n in array)
                {
                    var childData = ObjectToKeyValue(n);
                    data.AddRange(childData);
                }

            }
            else
            {
                var value = node.AsValue();
                var key = NodePathToKey(node.GetPath());
                data.Add(new KeyValuePair<string, string>(key, value.ToString()));

            }
            return data;
        }


    }
}
