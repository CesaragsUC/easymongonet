using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EasyMongoNet.Abstractions;

public interface IDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; }

    DateTime CreatedAt { get; set; }

    [BsonIgnoreIfNull]
    DateTime? ModifiedAt { get; set; }
}