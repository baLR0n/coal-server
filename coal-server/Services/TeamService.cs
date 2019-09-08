using COAL.CORE.Models;
using COAL.CORE.Models.Team;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CoalServer.Services
{
    public class TeamService
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

        public List<Team> Get() =>
            this.teams.Find(team => true).ToList();

        public Team Get(string id) =>
            this.teams.Find<Team>(team => team.TeamId == id).FirstOrDefault();

        public Team Create(Team team)
        {
            this.teams.InsertOne(team);
            return team;
        }

        public void Update(string id, Team teamIn) =>
            this.teams.ReplaceOne(team => team.Id == id, teamIn);

        public void Remove(Team teamIn) =>
            this.teams.DeleteOne(team => team.Id == teamIn.Id);

        public void Remove(string id) =>
            this.teams.DeleteOne(team => team.Id == id);

    }
}
