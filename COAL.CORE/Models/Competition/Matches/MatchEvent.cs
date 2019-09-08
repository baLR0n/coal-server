using MongoDB.Bson.Serialization.Attributes;

namespace COAL.CORE.Models.Competition.Matches
{
    public class MatchEvent : BaseModel
    {
        [BsonElement("match_event_id")]
        public int MatchEventId { get; set; }

        [BsonElement("minute")]
        public int Minute { get; set; }

        [BsonElement("stoppage_time")]
        public int StoppageTime { get; set; }

        [BsonElement("event_type")]
        public int EventType { get; set; }

        [BsonElement("player_id")]
        public int PlayerId { get; set; }

        [BsonElement("player2_id")]
        public int Player2Id { get; set; }
    }
}
