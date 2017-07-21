using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace WCFDemo.NetworkUtility
{
    public enum HttpRequestType
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class HttpHelper
    {

        public static TResult Get<TResult>(string url, string authorization = null) where TResult : class, new()
        {
            return RESTfulRequest<TResult>(url, HttpRequestType.GET, null, authorization, null);
        }

        public static TResult Put<TResult>(string url, IEnumerable<KeyValuePair<string, string>> nameValueCollection, string authorization) where TResult : class, new()
        {
            var formParam = BuildFormParamater(nameValueCollection);
            return RESTfulRequest<TResult>(url, HttpRequestType.PUT, formParam, authorization, "application/x-www-form-urlencoded");
        }

        public static TResult Post<TResult>(string url, object objToJson, string authorization = null) where TResult : class, new()
        {
            var bodyContent = JsonConvert.SerializeObject(objToJson);
            return Post<TResult>(url, bodyContent, authorization);
        }

        public static TResult Post<TResult>(string url, IEnumerable<KeyValuePair<string, string>> nameValueCollection, string authorization) where TResult : class, new()
        {
            var formParam = BuildFormParamater(nameValueCollection);
            return Post<TResult>(url, formParam, authorization, "application/x-www-form-urlencoded");
        }

        public static TResult Post<TResult>(string url, string input, string authorization = null, string contentType = "application/json") where TResult : class, new()
        {
            return RESTfulRequest<TResult>(url, HttpRequestType.POST, authorization, input, contentType);
        }

        public static TResult Delete<TResult>(string url, string authorization) where TResult : class, new()
        {
            return RESTfulRequest<TResult>(url, HttpRequestType.DELETE, null, authorization, null);
        }

        public static TResult RESTfulRequest<TResult>(string url, HttpRequestType method, string authorization = null, string input = null, string contentType = null) where TResult : class, new()
        {
            var responseResult = RESTfulRequest(url, method, input, authorization, contentType);
            if (string.IsNullOrEmpty(responseResult))
                return null;
            else
            {
                return JsonConvert.DeserializeObject<TResult>(responseResult);
            }
        }

        public static string RESTfulRequest(string url, HttpRequestType method, object bodyContent = null, string authorization = null, string contentType = null)
        {

            string responseStr = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method.ToString();
                request.ContentType = string.IsNullOrEmpty(contentType) ? "application/x-www-form-urlencoded" : contentType;
                request.Accept = "application/json";
                if (!string.IsNullOrEmpty(authorization))
                    request.Headers.Add("Authorization", authorization);
                // bodyContent is needed for POST and PUT method
                if (method == HttpRequestType.POST || method == HttpRequestType.PUT)
                {
                    if (bodyContent == null)
                    {
                        request.Abort();
                        throw new Exception("bodyContent is needed for POST and PUT method");
                    }
                    using (StreamWriter requestStream = new StreamWriter(request.GetRequestStream()))
                    {
                        requestStream.Write(bodyContent);
                    }
                }
                using (var response = request.GetResponse())
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    responseStr = reader.ReadToEnd();
                    reader.Close();
                }
                request.Abort();
            }
            catch (WebException ex)
            {
                if (ex.Response != null) // error message from service
                {
                    using (StreamReader reader = new StreamReader(ex.Response.GetResponseStream()))
                    {
                        responseStr = reader.ReadToEnd();
                    }
                    if (!string.IsNullOrEmpty(responseStr))
                    {
                        var httpRes = JsonConvert.DeserializeObject<HttpResult>(responseStr);
                        throw new Exception(httpRes.Message + "(" + httpRes.StatusCode + ")");
                    }
                    throw;
                }
                var socketException = ex.InnerException as SocketException;
                if (socketException != null) // usually network problem
                {
                    throw ex.InnerException;
                }
                throw;
            }
            catch (Exception)
            {
                throw;
            }
            return responseStr;
        }





        #region Tool Methods

        private static string Encode(string data)
        {
            if (String.IsNullOrEmpty(data))
            {
                return String.Empty;
            }
            // Escape spaces as '+'.
            return Uri.EscapeDataString(data).Replace("%20", "+");
        }

        private static string BuildFormParamater(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
        {
            if (nameValueCollection == null)
                return string.Empty;
            // Encode and concatenate data
            StringBuilder builder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in nameValueCollection)
            {
                if (builder.Length > 0)
                {
                    // Not first, add a seperator
                    builder.Append('&');
                }

                builder.Append(Encode(pair.Key));
                builder.Append('=');
                builder.Append(Encode(pair.Value));
            }
            return builder.ToString();
        }
        #endregion
    }


}
