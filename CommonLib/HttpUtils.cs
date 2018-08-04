using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace CommonLib
{
    public class HttpUtils
    {
        /// <summary>
        /// 发送HttpRequest
        /// </summary>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<string> SendRequestWithStringOut(HttpMethod method,string content,string url,Dictionary<string,string> headers=null)
        {
            var client=new HttpClient();
            var request=BuildRequest(method, url, content, headers);
            var response=await client.SendAsync(request);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<Stream> SendRequestWithStreamOut(HttpMethod method, string content, string url, Dictionary<string, string> headers = null)
        {
            var client = new HttpClient();
            var request = BuildRequest(method, url, content, headers);
            var response = await client.SendAsync(request);
            return await response.Content.ReadAsStreamAsync();
        }


        /// <summary>
        /// Build Http Request Message
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static HttpRequestMessage BuildRequest(HttpMethod method, string url,string content, Dictionary<string, string> headers=null)
        {
            var request = new HttpRequestMessage(method, url);
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            return request;
        }

        /// <summary>
        /// Post 并获取Json类型返回值
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<TResponse> PostAsJson<TRequest,TResponse>(string url, TRequest request)
        {
            var client=new HttpClient();
            var response=await client.PostAsJsonAsync(url, request);
            return await response.Content.ReadAsAsync<TResponse>();
        }

        /// <summary>
        /// Get 并获取Json结果
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<TResponse> GetAsJson<TResponse>(string url)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsAsync<TResponse>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>

        public static async Task<string> GetAsString(string url)
        {
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }
        
    }
}
