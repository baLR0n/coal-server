using MongoDB.Bson.Serialization.Attributes;

namespace COAL.CORE.Models.Team
{
    public class SheetPosition : BaseModel
    {
        [BsonElement("sheet_position_id")]
        public int SheetPositionId { get; set; }

        [BsonElement("position")]
        public int Position { get; set; }

        [BsonElement("player_id")]
        public string PlayerId { get; set; }
    }
}
