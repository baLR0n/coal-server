using COAL.CORE.Models;
using COAL.CORE.Models.Team;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoalServer.Services.Players
{
    public interface IPlayerService : ICrudService<Player>
    {
        Task<bool> ApplyTeamAssignmentsAsync(List<TeamAssignment> assignments);

        Task<List<Player>> GetFromTeamAsync(string teamId);
    }
}
