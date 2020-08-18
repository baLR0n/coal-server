using COAL.CORE.Models.Competition;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoalServer.Services.Competitions
{
    public interface ICompetitionService : ICrudService<Competition>
    {
        Task<List<Competition>> ApplyCompetitionEntriesAsync(List<CompetitionEntry> competitionEntries);
    }
}
