using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Qlt.MsDataverse.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Qlt.MsDataverse.Extensions;
using System.Net;

namespace Qlt.MsDataverse.Services
{
#pragma warning disable S101 // Types should be named in PascalCase
    public class CdsWebApiService : ICdsWebApiService, IDisposable
#pragma warning restore S101 // Types should be named in PascalCase
    {
        private readonly HttpClient httpClient;
        private readonly ILogger<CdsWebApiService> logger;

        public CdsWebApiService(HttpClient httpClient, ILogger<CdsWebApiService> logger)
        {
            this.httpClient = httpClient;
            this.logger = logger;
        }

        public void Delete(string path, Dictionary<string, List<string>> headers = null)
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string path, Dictionary<string, List<string>> headers = null)
        {
            throw new NotImplementedException();
        }

        public JToken Get(string path, Dictionary<string, List<string>> headers = null)
        {
            return GetAsync(path, headers).GetAwaiter().GetResult();
        }

        public T Get<T>(string path, Dictionary<string, List<string>> headers = null)
        {
            return GetAsync<T>(path, headers).GetAwaiter().GetResult();
        }

        public async Task<JToken> GetAsync(string path, Dictionary<string, List<string>> headers = null)
        {
            using var message = new HttpRequestMessage(HttpMethod.Get, path);
            if (headers != null)
            {
                foreach (KeyValuePair<string, List<string>> header in headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }
            }

            using var response = await SendAsync(message, HttpCompletionOption.ResponseContentRead);
            if (response.StatusCode != HttpStatusCode.NotModified)
            {
                return JToken.Parse(await response.Content.ReadAsStringAsync());
            }

            return null;
        }

        public async Task<T> GetAsync<T>(string path, Dictionary<string, List<string>> headers = null)
        {
            return (await GetAsync(path, headers)).ToObject<T>();
        }

        public void Patch(Uri uri, object body, Dictionary<string, List<string>> headers = null)
        {
            throw new NotImplementedException();
        }

        public Task PatchAsync(Uri uri, object body, Dictionary<string, List<string>> headers = null)
        {
            throw new NotImplementedException();
        }

        public JObject Post(string path, object body, Dictionary<string, List<string>> headers = null)
        {
            return PostAsync(path, body, headers).GetAwaiter().GetResult();
        }

        public async Task<JObject> PostAsync(string path, object body, Dictionary<string, List<string>> headers = null)
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new StringContent(JObject.FromObject(body).ToString(), Encoding.UTF8, "application/json")
            };

            if (headers != null)
            {
                foreach (KeyValuePair<string, List<string>> header in headers)
                {
                    message.Headers.Add(header.Key, header.Value);
                }
            }

            using HttpResponseMessage response = await SendAsync(message);
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            try
            {
                return JObject.Parse(content);
            }
            catch (JsonReaderException ex)
            {
                logger.LogError("Response content parsing failed", ex);
                return null;
            }
        }

        public Uri PostCreate(string entitySetName, object body)
        {
            throw new NotImplementedException();
        }

        public async Task<Uri> PostCreateAsync(string entitySetName, object body)
        {
            using var message = new HttpRequestMessage(HttpMethod.Post, entitySetName)
            {
                Content = new StringContent(JObject.FromObject(body).ToString())
            };
            message.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            using HttpResponseMessage response = await SendAsync(message);
            return new Uri(response.Headers.GetValues("OData-EntityId").FirstOrDefault());
        }

        public void Put(string path, string property, string value)
        {
            throw new NotImplementedException();
        }

        public Task PutAsync(string path, string property, string value)
        {
            throw new NotImplementedException();
        }

        #region private methods

        /// <summary>
        /// Sends all requests with retry capabilities
        /// </summary>
        /// <param name="request">The request to send</param>
        /// <param name="httpCompletionOption">Indicates if HttpClient operations should be considered completed either as soon as a response is available, or after reading the entire response message including the content.</param>
        /// <param name="retryCount">The number of retry attempts</param>
        /// <returns>The response for the request.</returns>
        private async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            HttpCompletionOption httpCompletionOption = HttpCompletionOption.ResponseHeadersRead)
        {
            HttpResponseMessage response;
            try
            {
                //The request is cloned so it can be sent again.
                response = await httpClient.SendAsync(request.Clone(), httpCompletionOption);
            }
            catch (Exception ex)
            {
                throw new ServiceException("Sending request exception", ex);
            }

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            throw ParseError(response);
            //else
            //{
            //    // Give up re-trying if exceeding the maxRetries
            //    if (++retryCount >= serviceConfig.MaxRetries)
            //    {
            //        throw ParseError(response);
            //    }

            //    int seconds;
            //    //Try to use the Retry-After header value if it is returned.
            //    if (response.Headers.Contains("Retry-After"))
            //    {
            //        seconds = int.Parse(response.Headers.GetValues("Retry-After").FirstOrDefault());
            //    }
            //    else
            //    {
            //        //Otherwise, use an exponential backoff strategy
            //        seconds = (int)Math.Pow(2, retryCount);
            //    }
            //    Thread.Sleep(TimeSpan.FromSeconds(seconds));

            //    return await SendAsync(request, httpCompletionOption, retryCount);
            //}
        }

        /// <summary>
        /// Parses the Web API error
        /// </summary>
        /// <param name="response">The response that failed.</param>
        /// <returns></returns>
        private ServiceException ParseError(HttpResponseMessage response)
        {
            int code = 0;
            string message = "no content returned",
                   content = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();

            if (content.Length > 0)
            {
                var errorObject = JObject.Parse(content);
                message = errorObject["error"]["message"].Value<string>();
                code = Convert.ToInt32(errorObject["error"]["code"].Value<string>(), 16);
            }
            int statusCode = (int)response.StatusCode;
            string reasonPhrase = response.ReasonPhrase;

            return new ServiceException(code, statusCode, reasonPhrase, message);
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReleaseClient();
            }
        }

        private void ReleaseClient()
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
            }
        }

        #endregion
    }
}
