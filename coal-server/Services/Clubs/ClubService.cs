using COAL.CORE.Models;
using COAL.CORE.Models.Team;
using MongoDB.Driver;

using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoalServer.Services.Clubs
{
    public class ClubService : IClubService
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

        /// <summary>
        /// Returns all clubs.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Club>> GetAsync()
        {
            return await this.clubs.FindAsync(player => true).Result.ToListAsync();
        }

        /// <summary>
        /// Returns a club with a specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Club> GetAsync(string id)
        {
            return await this.clubs.FindAsync<Club>(club => club.Id == id).Result.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new club entity.
        /// </summary>
        /// <param name="club"></param>
        /// <returns></returns>
        public async Task<Club> CreateAsync(Club club)
        {
            await this.clubs.InsertOneAsync(club);
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

        /// <summary>
        /// Updates a specific club entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="clubIn"></param>
        /// <returns></returns>
        public async Task UpdateAsync(string id, Club clubIn)
        {
            await this.clubs.ReplaceOneAsync(club => club.Id == id, clubIn);
        }

        /// <summary>
        /// Updates a list of clubs
        /// </summary>
        /// <param name="clubsIn"></param>
        /// <returns></returns>
        public async Task UpdateManyAsync(List<Club> clubsIn)
        {
            clubsIn.ForEach(async p => { await this.UpdateAsync(p.Id, p); });
        }

        /// <summary>
        /// Removes a club
        /// </summary>
        /// <param name="clubIn"></param>
        /// <returns></returns>
        public async Task RemoveAsync(Club clubIn)
        {
            await this.clubs.DeleteOneAsync(club => club.Id == clubIn.Id);
        }


        /// <summary>
        /// Remove the club with an specific id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveAsync(string id)
        {
            await this.clubs.DeleteOneAsync(club => club.Id == id);
        }
    }
}
