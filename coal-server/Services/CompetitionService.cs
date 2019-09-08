using COAL.CORE.Models;
using COAL.CORE.Models.Competition;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoalServer.Services
{
    public class CompetitionService
    {
        private IMongoDatabase database;
        private IMongoCollection<Competition> competitions;
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
        }

        /// <summary>
        /// Refresh the database connection of the service.
        /// </summary>
        public void Refresh()
        {
            this.ConnectToDb(settings);
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

        public void Update(string id, Competition competitionIn) =>
            this.competitions.ReplaceOne(competition => competition.Id == id, competitionIn);

        public void Remove(Competition competitionIn) =>
            this.competitions.DeleteOne(competition => competition.Id == competitionIn.Id);

        public void Remove(string id) =>
            this.competitions.DeleteOne(competition => competition.Id == id);

    }
}
