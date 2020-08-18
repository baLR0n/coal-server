using COAL.CORE.Models;
using COAL.CORE.Models.Team;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoalServer.Services.Teams
{
    public class TeamService : ITeamService
    {
        private IMongoDatabase database;
        private IMongoCollection<Team> teams;
        private ICoalDatabaseSettings settings;

        public TeamService(ICoalDatabaseSettings settings)
        {
            this.ConnectToDb(settings);
            this.settings = settings;
        }

        /// <summary>
        /// Connects to the selected saveGame database.
        /// </summary>
        /// <param name="settings"></param>
        private void ConnectToDb(ICoalDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            this.database = client.GetDatabase(settings.DatabaseName);

            this.teams = database.GetCollection<Team>(settings.TeamsCollectionName);
        }

        /// <summary>
        /// Refresh the database connection of the service.
        /// </summary>
        public void Refresh()
        {
            this.ConnectToDb(settings);
        }

        /// <summary>
        /// Returns all teams.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Team>> GetAsync()
        {
            return await this.teams.FindAsync(player => true).Result.ToListAsync();
        }

        /// <summary>
        /// Returns a team with a specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Team> GetAsync(string id)
        {
            return await this.teams.FindAsync<Team>(team => team.Id == id).Result.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new team entity.
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public async Task<Team> CreateAsync(Team team)
        {
            await this.teams.InsertOneAsync(team);
            return team;
        }

        /// <summary>
        /// Insert a list of teams.
        /// </summary>
        /// <param name="teams"></param>
        /// <returns></returns>
        public async Task<List<Team>> CreateManyAsync(List<Team> teams)
        {
            await this.teams.InsertManyAsync(teams);

            return teams;
        }

        /// <summary>
        /// Updates a specific team entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="teamIn"></param>
        /// <returns></returns>
        public async Task UpdateAsync(string id, Team teamIn)
        {
            await this.teams.ReplaceOneAsync(team => team.Id == id, teamIn);
        }

        /// <summary>
        /// Updates a list of teams
        /// </summary>
        /// <param name="teamsIn"></param>
        /// <returns></returns>
        public async Task UpdateManyAsync(List<Team> teamsIn)
        {
            teamsIn.ForEach(async p => { await this.UpdateAsync(p.Id, p); });
        }

        /// <summary>
        /// Removes a team
        /// </summary>
        /// <param name="teamIn"></param>
        /// <returns></returns>
        public async Task RemoveAsync(Team teamIn)
        {
            await this.teams.DeleteOneAsync(team => team.Id == teamIn.Id);
        }


        /// <summary>
        /// Remove the team with an specific id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveAsync(string id)
        {
            await this.teams.DeleteOneAsync(team => team.Id == id);
        }
    }
}
