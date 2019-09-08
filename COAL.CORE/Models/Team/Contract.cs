using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace COAL.CORE.Models.Team
{
    public class Contract : BaseModel
    {
        [BsonElement("contract_id")]
        public string ContractId { get; set; }

        [BsonElement("club_id")]
        public string ClubId { get; set; }

        [BsonElement("player_id")]
        public string PlayerId { get; set; }

        [BsonElement("valid_team_ids")]
        public List<int> ValidTeamIds { get; set; }

        [BsonElement("start_date")]
        public DateTime StartDate { get; set; }

        [BsonElement("end_date")]
        public DateTime EndDate { get; set; }

        [BsonElement("weekly_wage")]
        public int WeeklyWage { get; set; }

        [BsonElement("bonus")]
        public int Bonus { get; set; }

        [BsonElement("release_clause")]
        public int ReleaseClause { get; set; }
    }
}
