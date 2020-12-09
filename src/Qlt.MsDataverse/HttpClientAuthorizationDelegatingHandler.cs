using Qlt.MsDataverse.Services;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Qlt.MsDataverse
{
    public class HttpClientAuthorizationDelegatingHandler : DelegatingHandler
    {
        private readonly ITokenService tokenService;

        public HttpClientAuthorizationDelegatingHandler(ITokenService tokenService, HttpMessageHandler innerHandler) : base(innerHandler)
        {
            this.tokenService = tokenService;
        }

        /// <summary>
        /// Overrides the default HttpClient.SendAsync operation so that authentication can be done.
        /// </summary>
        /// <param name="request">The request to send</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected override Task<HttpResponseMessage> SendAsync(
                  HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.Authorization = GetAuthHeader();
            return base.SendAsync(request, cancellationToken);
        }

        private AuthenticationHeaderValue GetAuthHeader()
        {
            var token = tokenService.GetTokenWithClientCredential().GetAwaiter().GetResult();
            return new AuthenticationHeaderValue("Bearer", token);
        }
    }
}
