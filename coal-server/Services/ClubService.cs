using COAL.CORE.Models;
using COAL.CORE.Models.Team;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoalServer.Services
{
    public class ClubService
    {
        private IMongoDatabase database;
        private IMongoCollection<Club> clubs;
        private ICoalDatabaseSettings settings;

        public ClubService(ICoalDatabaseSettings settings)
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

            this.clubs = database.GetCollection<Club>(settings.ClubsCollectionName);
        }

        /// <summary>
        /// Refresh the database connection of the service.
        /// </summary>
        public void Refresh()
        {
            this.ConnectToDb(settings);
        }

        public List<Club> Get() =>
            this.clubs.Find(club => true).ToList();

        public Club Get(string id) =>
            this.clubs.Find<Club>(club => club.ClubId == id).FirstOrDefault();

        public Club Create(Club club)
        {
            this.clubs.InsertOne(club);
            return club;
        }

        /// <summary>
        /// Insert a list of clubs.
        /// </summary>
        /// <param name="clubs"></param>
        /// <returns></returns>
        public async Task<List<Club>> CreateManyAsync(List<Club> clubs)
        {
            await this.clubs.InsertManyAsync(clubs);

            return clubs;
        }

        public void Update(string id, Club clubIn) =>
            this.clubs.ReplaceOne(club => club.Id == id, clubIn);

        public void Remove(Club clubIn) =>
            this.clubs.DeleteOne(club => club.Id == clubIn.Id);

        public void Remove(string id) =>
            this.clubs.DeleteOne(club => club.Id == id);

    }
}
