using EasyMongoNet.Abstractions;

namespace EasyMongoNet.Settings;

public class MongoDbSettings : IMongoDbSettings
{
    public string? ConnectionString { get; set; }
    public string? DatabaseName { get; set; }
    public static string Section =>  "MongoDbSettings";

}
