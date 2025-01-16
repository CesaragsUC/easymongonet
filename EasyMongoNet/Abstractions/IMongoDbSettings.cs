namespace EasyMongoNet.Abstractions;

public interface IMongoDbSettings
{
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public static string? Section { get; }
}
