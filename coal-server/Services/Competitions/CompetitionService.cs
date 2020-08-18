using COAL.CORE.Models;
using COAL.CORE.Models.Competition;
using COAL.CORE.Models.Team;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoalServer.Services.Competitions
{
    public class CompetitionService : ICompetitionService
    {
        private IMongoDatabase database;
        private IMongoCollection<Competition> competitions;
        private IMongoCollection<Club> clubs;
        private ICoalDatabaseSettings settings;

        public CompetitionService(ICoalDatabaseSettings settings)
        {
            this.ConnectToDb(settings);
            this.settings = settings;
        }

        private void ConnectToDb(ICoalDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            this.database = client.GetDatabase(settings.DatabaseName);

            this.competitions = database.GetCollection<Competition>(settings.CompetitionsCollectionName);
            this.clubs = database.GetCollection<Club>(settings.ClubsCollectionName);
        }

        /// <summary>
        /// Refresh the database connection of the service.
        /// </summary>
        public void Refresh()
        {
            this.ConnectToDb(settings);
        }

        /// <summary>
        /// Apply a list of competition entries.
        /// </summary>
        /// <param name="competitionEntries"></param>
        /// <returns></returns>
        public async Task<List<Competition>> ApplyCompetitionEntriesAsync(List<CompetitionEntry> competitionEntries)
        {
            List<Competition> tempList = new List<Competition>();

            competitionEntries.ForEach(async e =>
            {
                // get competition
                var competition = this.competitions.Find(t => t.SourceId == e.CompetitionId).FirstOrDefault();

                if (competition != null)
                {
                    // get club / team
                    Club club = this.clubs.Find(c => c.SourceId == e.TeamId).FirstOrDefault();
                    if (club != null)
                    {
                        // assign
                        competition.Teams.Add(club.FirstTeamId);
                        tempList.Add(competition);
                        await this.UpdateAsync(competition.Id, competition);
                    }
                }
            });

            return await this.GetAsync();
        }

        /// <summary>
        /// Returns all competitions.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Competition>> GetAsync()
        {
            return await this.competitions.FindAsync(player => true).Result.ToListAsync();
        }

        /// <summary>
        /// Returns a competition with a specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Competition> GetAsync(string id)
        {
            return await this.competitions.FindAsync<Competition>(competition => competition.Id == id).Result.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new competition entity.
        /// </summary>
        /// <param name="competition"></param>
        /// <returns></returns>
        public async Task<Competition> CreateAsync(Competition competition)
        {
            await this.competitions.InsertOneAsync(competition);
            return competition;
        }

        /// <summary>
        /// Insert a list of competitions.
        /// </summary>
        /// <param name="competitions"></param>
        /// <returns></returns>
        public async Task<List<Competition>> CreateManyAsync(List<Competition> competitions)
        {
            await this.competitions.InsertManyAsync(competitions);

            return competitions;
        }

        /// <summary>
        /// Updates a specific competition entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="competitionIn"></param>
        /// <returns></returns>
        public async Task UpdateAsync(string id, Competition competitionIn)
        {
            await this.competitions.ReplaceOneAsync(competition => competition.Id == id, competitionIn);
        }

        /// <summary>
        /// Updates a list of competitions
        /// </summary>
        /// <param name="competitionsIn"></param>
        /// <returns></returns>
        public async Task UpdateManyAsync(List<Competition> competitionsIn)
        {
            competitionsIn.ForEach(async p => { await this.UpdateAsync(p.Id, p); });
        }

        /// <summary>
        /// Removes a competition
        /// </summary>
        /// <param name="competitionIn"></param>
        /// <returns></returns>
        public async Task RemoveAsync(Competition competitionIn)
        {
            await this.competitions.DeleteOneAsync(competition => competition.Id == competitionIn.Id);
        }


        /// <summary>
        /// Remove the competition with an specific id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveAsync(string id)
        {
            await this.competitions.DeleteOneAsync(competition => competition.Id == id);
        }

    }
}
