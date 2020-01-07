using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace COAL.CORE.Models
{
    public class BaseModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("database_position")]
        public int DatabasePosition { get; set; }

        [BsonElement("source_id")]
        public string SourceId { get; set; }
    }
}
