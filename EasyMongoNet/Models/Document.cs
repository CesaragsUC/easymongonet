using EasyMongoNet.Abstractions;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EasyMongoNet.Models;

public abstract class Document : IDocument
{
    protected Document()
    {
        Id = ObjectId.GenerateNewId().ToString();
    }
    public string Id { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [BsonIgnoreIfNull]
    public DateTime? ModifiedAt { get; set; }
}