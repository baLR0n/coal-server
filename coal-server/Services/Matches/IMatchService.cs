using COAL.CORE.Models.Competition;
using COAL.CORE.Models.Competition.Matches;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoalServer.Services.Matches
{
    public interface IMatchService : ICrudService<Match>
    {
        Task<List<Match>> GetFixturesForTableAsync(string tableId);

        Task<List<Match>> CreateFixturesFromCompetitionAsync(Competition competition);

        Task<List<Match>> SimulateMatchesAsync(List<Match> matches);

        Task<List<Match>> GetMatchesToSimulateAsync(DateTime inGameDate);
    }
}
