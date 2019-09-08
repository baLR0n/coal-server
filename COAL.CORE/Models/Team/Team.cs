using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace COAL.CORE.Models.Team
{
    public class Team : BaseModel
    {
        [BsonElement("team_id")]
        public string TeamId { get; set; }

        [BsonElement("club_id")]
        public string ClubId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("stadium_id")]
        public string StadiumId { get; set; }
    }
}
