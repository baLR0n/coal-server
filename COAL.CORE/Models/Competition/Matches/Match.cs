using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace COAL.CORE.Models.Competition.Matches
{
    public class Match : BaseModel
    {
        [BsonElement("match_id")]
        public string MatchId { get; set; }

        [BsonElement("table_id")]
        public string TableId { get; set; }

        [BsonElement("date_time")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime DateTime { get; set; }

        [BsonElement("game_day")]
        public int GameDay { get; set; }

        [BsonElement("home_team_id")]
        public string HomeTeamId { get; set; }

        [BsonElement("away_team_id")]
        public string AwayTeamId { get; set; }

        [BsonElement("stadium_id")]
        public string StadiumId { get; set; }

        [BsonElement("location")]
        public string Location { get; set; }

        [BsonElement("home_goals")]
        public int HomeGoals { get; set; }

        [BsonElement("away_goals")]
        public int AwayGoals { get; set; }

        [BsonElement("match_events")]
        public List<MatchEvent> MatchEvents { get; set; }
    }
}
