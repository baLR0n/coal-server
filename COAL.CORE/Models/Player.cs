using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace COAL.CORE.Models
{
    public class Player : BaseModel
    {
        [BsonElement("player_id")]
        public string PlayerId { get; set; }

        // Player meta data
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("shirt_name")]
        public string ShirtName { get; set; }

        [BsonElement("shirt_number")]
        public int ShirtNumber { get; set; }

        [BsonElement("birthday")]
        public DateTime Birthday { get; set; }

        [BsonElement("club_id")]
        public string ClubId { get; set; }

        [BsonElement("team_id")]
        public string TeamId { get; set; }

        [BsonElement("secondary_team_id")]
        public int SecondaryTeamId { get; set; }

        [BsonElement("national_team_id")]
        public int NationalTeamId { get; set; }

        // All time player stats
        [BsonElement("bio_id")]
        public int BioId { get; set; }

        // Player attributes
        [BsonElement("overall")]
        public int Overall { get; set; }

        [BsonElement("attacking_prowess")]
        public int AttackingProwess { get; set; }

        [BsonElement("ball_control")]
        public int BallControl { get; set; }

        [BsonElement("finishing")]
        public int Finishing { get; set; }

        [BsonElement("main_position")]
        public int MainPosition { get; set; }
    }
}
