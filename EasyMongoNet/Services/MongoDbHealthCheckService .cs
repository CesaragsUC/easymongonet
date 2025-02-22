using EasyMongoNet.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Polly;
using Polly.Retry;
using Serilog;

namespace EasyMongoNet.Services;

public class MongoDbHealthCheckService : BackgroundService
{
    private readonly IMongoClient _mongoClient;
    private readonly ILogger<MongoDbHealthCheckService> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly string _databaseName;
    private readonly TimeSpan _checkInterval = TimeSpan.FromSeconds(30);

    public MongoDbHealthCheckService(IOptions<MongoDbSettings> settings, ILogger<MongoDbHealthCheckService> logger)
    {
        _mongoClient = new MongoClient(settings.Value.ConnectionString);
        _databaseName = settings.Value.DatabaseName!;
        _logger = logger;

        _retryPolicy = Policy
            .Handle<MongoException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
                retryCount: 3,
                sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)), // Backoff exponencial
                onRetry: (exception, timeSpan, retryCount, context) =>
                {
                    Log.Error("[Polly] MongoDB Retry {RetryCount} - Waiting {TimeSpan} sec due to: {ExceptionMessage}", retryCount, timeSpan.TotalSeconds, exception.Message);
                });
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckMongoDbConnectionAsync();
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CheckMongoDbConnectionAsync()
    {
        try
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                var mongoClient = new MongoClient(new MongoClientSettings
                {
                    Server = _mongoClient.Settings.Server,
                    ConnectTimeout = TimeSpan.FromSeconds(3),
                    SocketTimeout = TimeSpan.FromSeconds(3)
                });

                var database = mongoClient.GetDatabase(_databaseName);

                var collections = await database.ListCollectionNamesAsync();
                await collections.FirstOrDefaultAsync();

                Log.Information("[MongoDbHealthCheck] MongoDB is reachable.");
            });
        }
        catch (Exception ex)
        {
            Log.Error(ex, "[MongoDbHealthCheck] MongoDB connection failed: {Message}", ex.Message);
        }
    }

}
