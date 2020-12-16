using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Qlt.MsDataverse.Services;
using Serilog;
using Serilog.Extensions.Logging;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Qlt.MsDataverse.Consoles
{
    class Program
    {
        public static readonly string BasePath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
        static Task Main(string[] args)
        {
            using IHost host = CreateHostBuilder(args).Build();
            using IServiceScope serviceScope = host.Services.CreateScope();
            IServiceProvider provider = serviceScope.ServiceProvider;
            provider.GetRequiredService<ApplicationStartup>().Run();
            return host.RunAsync();
        }

        static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureLogging(SetupLogging)
                .ConfigureServices((_, services) =>
                {
                    services.AddTransient<ApplicationStartup>();
                    services.AddSingleton<ITokenService, TokenService>(sp =>
                    {
                        var configurations = sp.GetService<IConfiguration>();
                        var connectionParams = configurations.GetSection("Api_Params").Value;
                        return new TokenService(new ServiceConfig(connectionParams));
                    });
                });

        private static void SetupLogging(ILoggingBuilder loggingBuilder)
        {
            var builder = new ConfigurationBuilder();
            BuildConfiguration(builder);
            var functionDependencyContext = DependencyContext.Load(typeof(ApplicationStartup).Assembly);
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Build(), sectionName: "Serilog", dependencyContext: functionDependencyContext)
                .CreateLogger();
        }

        private static readonly Action<IConfigurationBuilder> BuildConfiguration =
            builder => builder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
    }
}
