using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Http;
using Newtonsoft.Json;

namespace FileUpload
{
    /// <summary>
    /// Http辅助类
    /// </summary>
    public class HttpHelper
    {
        private static HttpHelper instance = null;
        private static TimeSpan RestTimeOut = new TimeSpan(0, 1, 0);
        private static object locker = new object();
        private HttpHelper() { }
        public static HttpHelper Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (locker)
                    {
                        return instance ?? (instance = new HttpHelper());
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 获取PJSON
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private HttpContent GetJsonContent(object t)
        {
            var strContent = Newtonsoft.Json.JsonConvert.SerializeObject(t);
            var data = Encoding.UTF8.GetBytes(strContent);
            return HttpContent.Create(data, "application/json");
        }

        #region Basic 

        /// <summary>
        /// 添加HttpHeader系in系
        /// </summary>
        /// <param name="client"></param>
        /// <param name="headers"></param>
        private void AppendHttpHeader(HttpClient client, Dictionary<string, string> headers)
        {
            if (headers != null && client != null)
            {
                foreach (var key in headers.Keys)
                {
                    client.DefaultHeaders.Add(key, headers[key]);
                }
            }
        }

        /// <summary>
        /// Post数据到指定Url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="requestBody"></param>
        /// <param name="headers"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public string PostDataTo(string url, string requestBody, Dictionary<string, string> headers = null, TimeSpan? timeOut=null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultHeaders.Add("Content-Type", "application/json");
                client.DefaultHeaders.Add("Accept", "application/json");
                AppendHttpHeader(client, headers);
                client.TransportSettings.ConnectionTimeout = timeOut.HasValue ? timeOut : RestTimeOut;
                var data = Encoding.UTF8.GetBytes(requestBody);
                var postContent = HttpContent.Create(data, "application/json");
                var response = client.Post(url, postContent);
                var content = response.Content.ReadAsString();

                //LoggerFactory.CreateLog().LogInfo("Request to {0} using methord(Post) with body {1} and Response is {2}.", url, string.IsNullOrEmpty(requestBody) ? "" : requestBody, content);
                try
                {
                    response.EnsureStatusIsSuccessful();
                }
                catch (Exception e)
                {
                    throw new Exception(content, e);
                }
                return content;
            }
        }

        /// <summary>
        /// Get Http返回结果
        /// </summary>
        /// <param name="url"></param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public string GetResponseFrom(string url, Dictionary<string, string> headers = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultHeaders.Add("Content-Type", "application/json");
                client.DefaultHeaders.Add("Accept", "application/json");
                AppendHttpHeader(client, headers);
                client.TransportSettings.ConnectionTimeout = RestTimeOut;
                var response = client.Get(url);

                var content = response.Content.ReadAsString();
                //LoggerFactory.CreateLog().LogInfo("Request to {0} using methord(Get) and Response is {1}.", url, content);
                try
                {
                    response.EnsureStatusIsSuccessful();
                }
                catch (Exception e)
                {
                    throw new Exception(content,e);
                }
                
                return content;
            }
        }

        #endregion

        /// <summary>
        /// 获取JSON返回结果
        /// </summary>
        /// <typeparam name="TResponseType"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public TResponseType GetJsonResult<TResponseType>(string url) where TResponseType : class
        {
            var response = GetResponseFrom(url);
            var result = JsonConvert.DeserializeObject<TResponseType>(response);
            return result;
        }

        /// <summary>
        /// Post数据并返回Json对象
        /// </summary>
        /// <typeparam name="TParamType"></typeparam>
        /// <typeparam name="TResponseType"></typeparam>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public TResponseType PostWithJsonResponse<TParamType, TResponseType>(string url, TParamType param) where TResponseType : class
        {
            var content = JsonConvert.SerializeObject(param);
            var response = PostDataTo(url, content, null, RestTimeOut);
            var result = JsonConvert.DeserializeObject<TResponseType>(response);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public string PostTo(string url, object param)
        {
            var content = JsonConvert.SerializeObject(param);
            return PostDataTo(url, content, null, RestTimeOut);
        }
    }
}
