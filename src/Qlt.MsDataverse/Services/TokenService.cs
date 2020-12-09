using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using Qlt.MsDataverse.Extensions;
using System.Threading.Tasks;
using LazyCache;

namespace Qlt.MsDataverse.Services
{
    public class TokenService : ITokenService
    {
        private readonly ServiceConfig config;
        private readonly IAppCache _cache = new CachingService();
        private readonly string KEYNAME = $"{typeof(TokenService).Namespace}.TokenService";

        public TokenService(ServiceConfig configParam)
        {
            Guard.ThrowIfNull(configParam, nameof(configParam));
            config = configParam;

            Guard.ThrowIfNull(config.ClientId, nameof(config.ClientId));
            Guard.ThrowIfNull(config.ClientSecrect, nameof(config.ClientSecrect));
        }

        public async Task<string> GetTokenWithClientCredential()
        {
            var token = _cache.Get<string>(KEYNAME);
            if (token.IsPresent())
            {
                return token;
            }

            var credential = new ClientCredential(config.ClientId, config.ClientSecrect);
            var authContext = new AuthenticationContext(config.Authority);
            var authResult = await authContext.AcquireTokenAsync(config.Url, credential);
            if (authResult.AccessToken.IsPresent())
            {
                _cache.Add(KEYNAME, authResult.AccessToken, authResult.ExpiresOn.AddMinutes(-5));
            }

            return authResult.AccessToken;
        }
    }
}
