using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace COAL.CORE.Models.Competition
{
    public class Table : BaseModel
    {
        [BsonElement("table_id")]
        public string TableId { get; set; }

        [BsonElement("competition_id")]
        public string CompetitionId { get; set; }

        public List<TableEntry> Teams { get; set; } = new List<TableEntry>();
    }
}
