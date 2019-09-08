using COAL.CORE;
using COAL.CORE.Models;
using COAL.PES.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace COAL.PES
{
    public static class PlayerConverter
    {
        public static Task<Player> Convert(PESPlayer sourcePlayer)
        {
            Player converted = new Player()
            {
                PlayerId = Guid.NewGuid().ToString(),
                SourceId = sourcePlayer.Id.ToString(),
                FirstName = "",
                LastName = sourcePlayer.Name,
                AttackingProwess = System.Convert.ToInt32(sourcePlayer.AttackProwess),
                BallControl = System.Convert.ToInt32(sourcePlayer.BallControl),
                Finishing = System.Convert.ToInt32(sourcePlayer.Finishing),
            };

            return Task.FromResult(converted);
        }

        public static async Task<List<Player>> ConvertMany(List<PESPlayer> sourcePlayerList)
        {
            List<Player> result = new List<Player>();

            foreach (PESPlayer player in sourcePlayerList)
            {
                result.Add(await Convert(player));
            }

            return result;
        }
    }
}
