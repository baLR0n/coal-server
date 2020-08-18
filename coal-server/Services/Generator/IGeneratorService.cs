using COAL.CORE.Models.Competition;
using COAL.CORE.Models.Team;
using COAL.CORE.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoalServer.Services.Generator
{
    public interface IGeneratorService
    {
        void Refresh();

        Task<List<Competition>> GenerateCompetitionsAsync();

        Task<List<Club>> GenerateClubsAsync();

        Task<List<Team>> GenerateTeamsAsync(List<Club> clubs, ClubGenerationMode mode = ClubGenerationMode.AllTeams);

        Task<List<Player>> GeneratePlayersAsync(List<Club> clubs, ClubGenerationMode mode = ClubGenerationMode.AllTeams);

        Task<List<Contract>> GenerateContractsAsync(List<Player> players);
    }
}
