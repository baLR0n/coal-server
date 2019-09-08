using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace COAL.CORE.Models
{
    public class Bio : BaseModel
    {
        [BsonElement("bio_id")]
        public string BioId { get; set; }

        [BsonElement("matches")]
        public List<string> Matches { get; set; }

        [BsonElement("contracts")]
        public List<string> Contracts { get; set; }

        [BsonElement("transfers")]
        public List<string> Transfers { get; set; }

        [BsonElement("titles")]
        public List<string> Titles { get; set; }

    }
}
