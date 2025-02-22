using EasyMongoNet.Abstractions;
using EasyMongoNet.Repository;
using EasyMongoNet.Services;
using EasyMongoNet.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyMongoNet.Exntesions;

public static class ServiceCollectionExtension
{
    public static void AddEasyMongoNet(this IServiceCollection services,
        IConfiguration configuration,
        int? healthCheck = null)
    {
        // Validate parameters
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        // Configure MongoDbSettings
        services.Configure<MongoDbSettings>(options =>
        {
            configuration.GetSection(MongoDbSettings.Section).Bind(options);
        });

        // Register MongoRepository
        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

        // Add health check service if required
        if (healthCheck.HasValue && healthCheck.Value == (int)HealthCheckOptions.Active)
        {
            services.AddHostedService<MongoDbHealthCheckService>();
        }
    }
}