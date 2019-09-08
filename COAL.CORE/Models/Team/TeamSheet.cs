using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace COAL.CORE.Models.Team
{
    public class TeamSheet : BaseModel
    {
        [BsonElement("team_sheet_id")]
        public int TeamSheetId { get; set; }

        [BsonElement("team_id")]
        public int TeamId { get; set; }

        [BsonElement("sheet_positions")]
        public List<SheetPosition> SheetPositions { get; set; }

        [BsonElement("formation_id")]
        public int FormationId { get; set; }
    }
}
