using MongoDB.Bson.Serialization.Attributes;

namespace COAL.CORE.Models.Competition
{
    public class TableEntry : BaseModel
    {
        [BsonElement("table_entry_id")]
        public string TableEntryId { get; set; }

        [BsonElement("table_id")]
        public string TableId { get; set; }

        [BsonElement("team_id")]
        public string TeamId { get; set; }

        [BsonElement("team_name")]
        public string TeamName { get; set; }

        [BsonElement("games_played")]
        public int GamesPlayed { get; set; } // ?

        [BsonElement("games_won")]
        public int GamesWon { get; set; }

        [BsonElement("games_drawn")]
        public int GamesDrawn { get; set; }

        [BsonElement("games_lost")]
        public int GamesLost { get; set; }

        [BsonElement("goals_scored")]
        public int GoalsScored { get; set; }

        [BsonElement("goals_allowed")]
        public int GoalsAllowed { get; set; }
    }
}
