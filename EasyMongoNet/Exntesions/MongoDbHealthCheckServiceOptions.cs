using System.ComponentModel;

namespace EasyMongoNet.Exntesions;

public enum HealthCheckOptions
{
    [Description("Check MongoDB connection")]
    Active,
    [Description("Do not check MongoDB connection")]
    Inactive
}