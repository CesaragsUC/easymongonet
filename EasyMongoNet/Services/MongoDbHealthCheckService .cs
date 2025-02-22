using EasyMongoNet.Settings;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using Polly;
using Polly.Retry;

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
                    logger.LogWarning($"[Polly] MongoDB Retry {retryCount} - Waiting {timeSpan.TotalSeconds} sec due to: {exception.Message}");
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
                var database = _mongoClient.GetDatabase(_databaseName);
                await database.RunCommandAsync((Command<BsonDocument>)"{ping:1}");
                _logger.LogInformation("[MongoDbHealthCheck] MongoDB is reachable.");
            });
        }
        catch (Exception ex)
        {
            _logger.LogError("[MongoDbHealthCheck] MongoDB connection failed: {Message}", ex.Message);
        }
    }
}
