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
        /// <summary>
        /// Converts a PES player to the DB format.
        /// </summary>
        /// <param name="sourcePlayer"></param>
        /// <returns></returns>
        public static Task<Player> Convert(PESPlayer sourcePlayer)
        {
            Player converted = new Player()
            {
                DatabasePosition = sourcePlayer.Position,
                PlayerId = Guid.NewGuid().ToString(),
                SourceId = sourcePlayer.Id.ToString(),
                Name = sourcePlayer.Name,
                ShirtName = sourcePlayer.ShirtName,
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

        /// <summary>
        /// Convert a DB-Player to the PES format.
        /// </summary>
        /// <param name="sourcePlayer"></param>
        /// <returns></returns>
        public static Task<PESPlayer> ConvertBack(Player sourcePlayer)
        {
            PESPlayer converted = new PESPlayer()
            {
                Position = sourcePlayer.DatabasePosition,
                Id = System.Convert.ToUInt32(sourcePlayer.SourceId),
                Name = sourcePlayer.Name,
                ShirtName = sourcePlayer.ShirtName,
                AttackProwess = System.Convert.ToUInt32(sourcePlayer.AttackingProwess),
                BallControl = System.Convert.ToUInt32(sourcePlayer.BallControl),
                Finishing = System.Convert.ToUInt32(sourcePlayer.Finishing),
            };

            return Task.FromResult(converted);
        }

        public static async Task<List<PESPlayer>> ConvertBackMany(List<Player> sourcePlayerList)
        {
            List<PESPlayer> result = new List<PESPlayer>();

            foreach (Player player in sourcePlayerList)
            {
                result.Add(await ConvertBack(player));
            }

            return result;
        }
    }
}
