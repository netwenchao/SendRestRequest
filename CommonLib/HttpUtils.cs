using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CommonLib
{
    public interface ILog
    {
        void Info(string msg);
        void Error(string errMsg);
        void Error(Exception exp);
        void Warning(string msg);
    }

    public class HttpUtils
    {
        #region core
        /// <summary>
        /// 发送HttpRequest
        /// </summary>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<string> SendRequest(HttpMethod method, string content, string url, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                var request = BuildRequest(method, url, content, headers);
                var response = await client.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="content"></param>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<Stream> SendRequestStream(HttpMethod method, string content, string url, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                var request = BuildRequest(method, url, content, headers);
                var response = await client.SendAsync(request);
                return await response.Content.ReadAsStreamAsync();
            }
        }


        /// <summary>
        /// Build Http Request Message
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static HttpRequestMessage BuildRequest(HttpMethod method, string url, string content, Dictionary<string, string> headers = null, string contentType = "application/json")
        {
            var request = new HttpRequestMessage() { Method = method };
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            request.RequestUri = new Uri(url);
            if (string.IsNullOrWhiteSpace(contentType))
            {
                request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            }
            if (!string.IsNullOrEmpty(content))
            {
                request.Content = new StringContent(content);
            }
            return request;
        }

        #endregion

        /// <summary>
        /// Post 并获取Json类型返回值
        /// </summary>
        /// <typeparam name="TRequest"></typeparam>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public static async Task<TResponse> PostAsJson<TRequest, TResponse>(string url, TRequest request, Dictionary<string, string> header)
        {
            var response = SendRequest(HttpMethod.Post, JsonConvert.SerializeObject(request), url, header);
            var responseText = await response;
            return JsonConvert.DeserializeObject<TResponse>(responseText);
        }

        /// <summary>
        /// Get 并获取Json结果
        /// </summary>
        /// <typeparam name="TResponse"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<TResponse> GetAsJson<TResponse>(string url)
        {
            var response = SendRequest(HttpMethod.Get, null, url);
            var responseText = await response;
            return JsonConvert.DeserializeObject<TResponse>(responseText);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>

        public static async Task<string> GetAsString(string url)
        {
            return await SendRequest(HttpMethod.Get, null, url);
        }
    }

    public sealed class PercentEncoding
    {
        public static string GenerateSignature(string url, string method, string requestBody, string appSecret, StringBuilder log = null, bool incHost = true)
        {
            var combined = new List<string>();
            // request method
            combined.Add(method.ToUpper());
            Uri uri = new Uri(url);
            if (incHost)
            {
                // scheme
                combined.Add(uri.Scheme.ToLower());
                // host
                combined.Add(uri.Host.ToLower());
                // port
                combined.Add(uri.Port.ToString());
            }
            // path
            string path = uri.AbsolutePath.ToLower();
            path = path.Replace("\\", "/");
            if (path.EndsWith("/"))
                path = path.Substring(0, path.Length - 1);
            combined.Add(PercentEncoding.Encode(path));

            // query string
            string q = (uri.Query ?? "").Trim();
            if (q.Length > 0)
            {
                if (q.StartsWith("?"))
                    q = q.Substring(1);
                string[] itemStrs = q.Split(new char[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
                List<KeyValuePair<string, string>> items = new List<KeyValuePair<string, string>>();
                foreach (string itemStr in itemStrs)
                {
                    if (itemStr.Trim().Length == 0) continue;
                    string key = "", value = "";

                    int index = itemStr.IndexOf("=");
                    if (index <= 0) // = is missing or key is missing, ignore
                    {
                        continue;
                    }
                    else
                    {
                        key = HttpUtility.UrlDecode(itemStr.Substring(0, index)).Trim().ToLower();
                        value = HttpUtility.UrlDecode(itemStr.Substring(index + 1)).Trim();
                        items.Add(new KeyValuePair<string, string>(key, value));
                    }
                }

                // query
                combined.Add(String.Join("&", items.OrderBy(t => t.Key).Select(t => String.Format("{0}={1}", PercentEncoding.Encode(t.Key), PercentEncoding.Encode(t.Value))).ToArray()));
            }
            else
                combined.Add("");

            // body
            combined.Add(PercentEncoding.Encode(requestBody ?? ""));
            // salt
            combined.Add(appSecret);

            string baseString = String.Join("|", combined.ToArray());
            if (log != null)
                log.AppendLine("Base String: " + baseString);

            System.Security.Cryptography.SHA256Managed s256 = new System.Security.Cryptography.SHA256Managed();
            byte[] buff;
            buff = s256.ComputeHash(Encoding.UTF8.GetBytes(baseString));
            s256.Clear();
            return Convert.ToBase64String(buff);
        }

        public static string Encode(string s)
        {
            string t = HttpUtility.UrlEncode(s);
            t = t.Replace("+", "%20");
            t = t.Replace("!", "%21");
            t = t.Replace("(", "%28");
            t = t.Replace(")", "%29");
            t = t.Replace("*", "%2a");
            return t;
        }
        public static string Decode(string s)
        {
            return HttpUtility.UrlDecode(s);
        }
    }
}
