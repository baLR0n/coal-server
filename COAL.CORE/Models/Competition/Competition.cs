using COAL.CORE.Models.Competition.Matches;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace COAL.CORE.Models.Competition
{
    public class Competition : BaseModel
    {
        [BsonElement("competition_id")]
        public string CompetitionId { get; set; }

        [BsonElement("competition_type_id")]
        public string CompetitionTypeId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("country_id")]
        public int CountryId { get; set; }

        [BsonElement("region_id")]
        public int RegionId { get; set; }

        [BsonElement("teams")]
        public List<string> Teams { get; set; }

        [BsonElement("fixtures")]
        public List<Match> Fixtures { get; set; }

        [BsonElement("season")]
        public int Season { get; set; }

        [BsonElement("level")]
        public int Level { get; set; }

        [BsonElement("relegation_mode_id")]
        public int RelegationModeId { get; set; }

        [BsonElement("has_playoffs")]
        public bool HasPlayoffs { get; set; }

        [BsonElement("start_date")]
        public DateTime StartDate { get; set; }

        [BsonElement("end_date")]
        public DateTime EndDate { get; set; }

        [BsonElement("winner_id")]
        public string WinnerId { get; set; }
    }
}
