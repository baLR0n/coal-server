using MongoDB.Bson.Serialization.Attributes;

namespace COAL.CORE.Models.Competition
{
    public class Cup : Competition
    {
        [BsonElement("cup_ruleset_id")]
        public int CupRulesetId { get; set; }
    }
}
