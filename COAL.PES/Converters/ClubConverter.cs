using COAL.CORE;
using COAL.CORE.Models;
using COAL.CORE.Models.Team;
using COAL.PES.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace COAL.PES
{
    public static class ClubConverter
    {
        /// <summary>
        /// Converts a PES team to the DB format.
        /// </summary>
        /// <param name="sourcePlayer"></param>
        /// <returns></returns>
        public static Task<Club> Convert(PESTeam sourceTeam)
        {
            var guid = Guid.NewGuid().ToString();
            Club converted = new Club()
            {
                ClubId = guid,
                FirstTeamId = guid.ToString() + "T1",
                ReservesTeamId = guid.ToString() + "T2",
                YouthTeamId = guid.ToString() + "T3",
                SourceId = sourceTeam.Id.ToString(),
                DatabasePosition = sourceTeam.Position,
                Name = sourceTeam.Name,
                CountryId = (int)sourceTeam.CountryId,
                IsNationalTeam = sourceTeam.National == 1

            };

            return Task.FromResult(converted);
        }

        public static async Task<List<Club>> ConvertMany(List<PESTeam> sourceTeamList)
        {
            List<Club> result = new List<Club>();

            foreach (PESTeam team in sourceTeamList)
            {
                result.Add(await Convert(team));
            }

            return result;
        }

        /// <summary>
        /// Convert a DB-Club to the PES format.
        /// </summary>
        /// <param name="sourcePlayer"></param>
        /// <returns></returns>
        public static Task<PESTeam> ConvertBack(Club sourceClub)
        {
            PESTeam converted = new PESTeam()
            {
                Position = sourceClub.DatabasePosition,
                Id = System.Convert.ToUInt32(sourceClub.SourceId),
                Name = sourceClub.Name
            };

            return Task.FromResult(converted);
        }

        public static async Task<List<PESTeam>> ConvertBackMany(List<Club> sourceClubList)
        {
            List<PESTeam> result = new List<PESTeam>();

            foreach (Club club in sourceClubList)
            {
                result.Add(await ConvertBack(club));
            }

            return result;
        }
    }
}
