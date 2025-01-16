using EasyMongoNet.Abstractions;
using EasyMongoNet.Repository;
using EasyMongoNet.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EasyMongoNet.Exntesions;

public static class ServiceCollectionExtension
{
    public static void AddEasyMongoNet(this IServiceCollection services, IConfiguration configuration)
    {

        services.Configure<MongoDbSettings>(options =>
        {
            configuration.GetSection(MongoDbSettings.Section).Bind(options);
        });

        services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

    }

}