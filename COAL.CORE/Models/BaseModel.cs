using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace COAL.CORE.Models
{
    public class BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
    }
}
