using COAL.CORE;
using COAL.CORE.Models;
using COAL.CORE.Models.Competition;
using COAL.CORE.Models.Team;
using COAL.PES.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace COAL.PES
{
    public static class CompetitionConverter
    {
        /// <summary>
        /// Converts a PES team to the DB format.
        /// </summary>
        /// <param name="sourcePlayer"></param>
        /// <returns></returns>
        public static Task<Competition> Convert(PESCompetition sourceCompetition)
        {
            var guid = Guid.NewGuid().ToString();
            Competition converted = new Competition()
            {
                CompetitionId = guid,
                SourceId = sourceCompetition.Id.ToString(),
                DatabasePosition = sourceCompetition.Position,
                Name = sourceCompetition.Name,
                Teams = new List<string>(),
                StartDate = DateTime.Today
            };

            return Task.FromResult(converted);
        }

        public static async Task<List<Competition>> ConvertMany(List<PESCompetition> sourceCompetitionList)
        {
            List<Competition> result = new List<Competition>();

            foreach (PESCompetition competition in sourceCompetitionList)
            {
                result.Add(await Convert(competition));
            }

            return result;
        }

        /// <summary>
        /// Convert a DB-Competition to the PES format.
        /// </summary>
        /// <param name="sourcePlayer"></param>
        /// <returns></returns>
        public static Task<PESCompetition> ConvertBack(Competition sourceCompetition)
        {
            PESCompetition converted = new PESCompetition()
            {
                Position = sourceCompetition.DatabasePosition,
                Id = System.Convert.ToUInt32(sourceCompetition.SourceId),
                Name = sourceCompetition.Name
            };

            return Task.FromResult(converted);
        }

        public static async Task<List<PESCompetition>> ConvertBackMany(List<Competition> sourceCompetitionList)
        {
            List<PESCompetition> result = new List<PESCompetition>();

            foreach (Competition competition in sourceCompetitionList)
            {
                result.Add(await ConvertBack(competition));
            }

            return result;
        }
    }
}
