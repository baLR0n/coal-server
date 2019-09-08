using COAL.CORE.Models;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace COAL.CORE.Core.Game
{
    public class SaveGame : BaseModel
    {
        [BsonElement("save_game_id")]
        public string SaveGameId { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("team_ids")]
        public List<int> TeamIds { get; set; }

        [BsonElement("ingame_date")]
        public DateTime InGameDate { get; set; }
    }
}
