using MongoDB.Bson.Serialization.Attributes;

namespace COAL.CORE.Models.Team
{
    public class Club : BaseModel
    {
        [BsonElement("club_id")]
        public string ClubId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("city")]
        public string City { get; set; }

        [BsonElement("country_id")]
        public int CountryId { get; set; }

        [BsonElement("region_id")]
        public int RegionId { get; set; }

        [BsonElement("balance")]
        public int Balance { get; set; }

        [BsonElement("dom_prestige")]
        public int DomesticPrestige { get; set; }

        [BsonElement("int_prestige")]
        public int InternationalPrestige { get; set; }

        [BsonElement("first_team_id")]
        public string FirstTeamId { get; set; }

        [BsonElement("reserves_team_id")]
        public string ReservesTeamId { get; set; }

        [BsonElement("youth_team_id")]
        public string YouthTeamId { get; set; }

        [BsonElement("is_national_team")]
        public bool IsNationalTeam { get; set; }
    }
}
