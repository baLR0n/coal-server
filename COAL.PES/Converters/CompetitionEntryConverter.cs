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
    public static class CompetitionEntryConverter
    {
        /// <summary>
        /// Converts a PES team to the DB format.
        /// </summary>
        /// <param name="sourceCompetitionEntry"></param>
        /// <returns></returns>
        public static Task<CompetitionEntry> Convert(PESCompetitionEntry sourceCompetitionEntry)
        {
            var guid = Guid.NewGuid().ToString();
            CompetitionEntry converted = new CompetitionEntry()
            {
                CompetitionEntryId = guid,
                SourceId = sourceCompetitionEntry.Id.ToString(),
                DatabasePosition = sourceCompetitionEntry.Position,
                TeamId = sourceCompetitionEntry.TeamId.ToString(),
                CompetitionId = sourceCompetitionEntry.CompetitionId.ToString(),
                Group = (int)sourceCompetitionEntry.Group,
                Order = sourceCompetitionEntry.Order
            };

            return Task.FromResult(converted);
        }

        public static async Task<List<CompetitionEntry>> ConvertMany(List<PESCompetitionEntry> sourceCompetitionEntryList)
        {
            List<CompetitionEntry> result = new List<CompetitionEntry>();

            foreach (PESCompetitionEntry competitionEntry in sourceCompetitionEntryList)
            {
                result.Add(await Convert(competitionEntry));
            }

            return result;
        }

        /// <summary>
        /// Convert a DB-CompetitionEntry to the PES format.
        /// </summary>
        /// <param name="sourceCompetitionEntry"></param>
        /// <returns></returns>
        public static Task<PESCompetitionEntry> ConvertBack(CompetitionEntry sourceCompetitionEntry)
        {
            PESCompetitionEntry converted = new PESCompetitionEntry()
            {
                Position = sourceCompetitionEntry.DatabasePosition,
                Id = System.Convert.ToUInt32(sourceCompetitionEntry.SourceId),
                TeamId = System.Convert.ToUInt32(sourceCompetitionEntry.TeamId),
                CompetitionId = System.Convert.ToUInt32(sourceCompetitionEntry.CompetitionId),
                Group = (uint)sourceCompetitionEntry.Group,
                Order = (byte)sourceCompetitionEntry.Order
            };

            return Task.FromResult(converted);
        }

        public static async Task<List<PESCompetitionEntry>> ConvertBackMany(List<CompetitionEntry> sourceCompetitionEntryList)
        {
            List<PESCompetitionEntry> result = new List<PESCompetitionEntry>();

            foreach (CompetitionEntry competitionEntry in sourceCompetitionEntryList)
            {
                result.Add(await ConvertBack(competitionEntry));
            }

            return result;
        }
    }
}
