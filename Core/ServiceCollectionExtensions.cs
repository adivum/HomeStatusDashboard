using Core.Interfaces;
using Core.Options;
using Core.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Core
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds HomeAssistant services to the DI container.
        /// If no configuration is provided, it automatically loads from appsettings.json and user-secrets.
        /// </summary>
        public static IServiceCollection AddHomeAssistantCore(this IServiceCollection services, IConfiguration? configuration = null)
        {
            // If no configuration provided, build one from Core's resources
            if (configuration == null)
            {
                var configBuilder = new ConfigurationBuilder();
                var coreAssemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var appsettingsPath = Path.Combine(coreAssemblyPath ?? "", "appsettings.json");
                
                configBuilder.AddJsonFile(appsettingsPath, optional: true, reloadOnChange: false);
                
                // Load user-secrets (using Core's UserSecretsId)
                configBuilder.AddUserSecrets(typeof(ServiceCollectionExtensions).Assembly, optional: true);
                
                configuration = configBuilder.Build();
            }
            
            services.Configure<HomeAssistantOptions>(configuration.GetSection(HomeAssistantOptions.SectionName));
            services.AddHttpClient<IHomeAssistantService, HomeAssistantService>();
            
            return services;
        }
    }
}
