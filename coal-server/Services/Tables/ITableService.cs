using System.Collections.Generic;
using System.Threading.Tasks;
using COAL.CORE.Models.Competition;
using COAL.CORE.Models.Competition.Matches;

namespace CoalServer.Services.Tables
{
    public interface ITableService : ICrudService<Table>
    {
        Task<List<Table>> ApplyMatchesAsync(List<Match> matches);
        Task<Table> CreateFromCompetitionAsync(Competition competition);
        Task<Table> GetFromCompetitonIdAsync(string competitionId);
    }
}