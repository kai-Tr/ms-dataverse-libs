using Qlt.MsDataverse.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Qlt.MsDataverse
{
    public static class DataverseServiceCollection
    {
        /// <summary>
        /// Configure Ms Dataverse services for the application.
        /// </summary>
        /// <param name="services">The IServiceCollection<c/> instance the extension method applies to.</param>
        /// <returns>An new instance of DataverseBuilder.</returns>
        public static DataverseBuilder AddDataverse(this IServiceCollection services, string connectionString)
        {
            var serviceConfig = new ServiceConfig(connectionString);
            services.AddOptions<ServiceConfig>();
            services.ConfigureOptions(serviceConfig);

            services.AddSingleton<ITokenService>((sp) =>
            {
                var option = sp.GetService<IOptions<ServiceConfig>>();
                Guard.ThrowIfNull(option, nameof(option));
                return new TokenService(option.Value);
            });

            return new DataverseBuilder(services);
        }
    }
}
