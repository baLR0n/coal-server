using COAL.CORE.Models;
using COAL.CORE.Models.Competition;
using COAL.CORE.Models.Team;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoalServer.Services
{
    public class CompetitionService
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

            return await Task.FromResult(this.Get());
        }

        public List<Competition> Get() =>
            this.competitions.Find(competition => true).ToList();

        public Competition Get(string id) =>
            this.competitions.Find<Competition>(competition => competition.Id == id).FirstOrDefault();

        public Competition Create(Competition competition)
        {
            this.competitions.InsertOne(competition);
            return competition;
        }

        /// <summary>
        /// Insert a list of competitions.
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public async Task<List<Competition>> CreateManyAsync(List<Competition> competitions)
        {
            await this.competitions.InsertManyAsync(competitions);

            return competitions;
        }

        public async Task UpdateAsync(string id, Competition competitionIn) =>
            await this.competitions.ReplaceOneAsync(competition => competition.Id == id, competitionIn);

        /// <summary>
        /// Updates a list of competitions
        /// </summary>
        /// <param name="competitionsIn"></param>
        /// <returns></returns>
        public void UpdateMany(List<Competition> competitionsIn)
        {
            competitionsIn.ForEach(async c => { await this.UpdateAsync(c.Id, c); });
        }

        public void Remove(Competition competitionIn) =>
            this.competitions.DeleteOne(competition => competition.Id == competitionIn.Id);

        public void Remove(string id) =>
            this.competitions.DeleteOne(competition => competition.Id == id);

    }
}
