using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using Qlt.MsDataverse.Services;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Qlt.MsDataverse
{
    public class DataverseBuilder
    {
        internal readonly IServiceCollection services;

        public DataverseBuilder(IServiceCollection services)
        {
            this.services = services;
        }

        /// <summary>
        /// setup http client with client credential auth
        /// </summary>
        /// <returns></returns>
        public DataverseBuilder WithClientCredential()
        {
            services.AddHttpClient<ICdsWebApiService>(nameof(CdsWebApiService),
                    (sp, httpClient) =>
                    {
                        var option = sp.GetService<IOptions<ServiceConfig>>();
                        Guard.ThrowIfNull(option, nameof(option));
                        var serviceConfig = option.Value;
                        var tokenService = sp.GetRequiredService<ITokenService>();
                        HttpMessageHandler messageHandler = new HttpClientAuthorizationDelegatingHandler(tokenService, new HttpClientHandler() { UseCookies = false });
                        httpClient = new HttpClient(messageHandler)
                        {
                            BaseAddress = new Uri(serviceConfig.Url + $"/api/data/v{serviceConfig.Version}/")
                        };

                        httpClient.BaseAddress = new Uri(serviceConfig.Url + $"/api/data/v{serviceConfig.Version}/");
                        httpClient.Timeout = TimeSpan.FromSeconds(serviceConfig.TimeoutInSeconds);
                        httpClient.DefaultRequestHeaders.Add("OData-MaxVersion", "4.0");
                        httpClient.DefaultRequestHeaders.Add("OData-Version", "4.0");
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                        if (serviceConfig.CallerObjectId != Guid.Empty)
                        {
                            httpClient.DefaultRequestHeaders.Add("CallerObjectId", serviceConfig.CallerObjectId.ToString());
                        }
                    })
                    .AddPolicyHandler(GetRetryPolicy());

            return this;
        }

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                  .HandleTransientHttpError()
                  .OrResult(response =>
                  {
                      return (int)response.StatusCode == 429;
                  })
                  .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        }
    }
}
