using COAL.CORE.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoalServer.Services
{
    public class PlayerService
    {
        private IMongoCollection<Player> players;
        private readonly ICoalDatabaseSettings settings;

        public PlayerService(ICoalDatabaseSettings settings)
        {
            this.ConnectToDb(settings);
            this.settings = settings;
        }

        private void ConnectToDb(ICoalDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            this.players = database.GetCollection<Player>(settings.PlayersCollectionName);
        }

        public void Refresh()
        {
            this.ConnectToDb(settings);
        }

        public List<Player> Get() =>
            
            this.players.Find(player => true).ToList();

        public Player Get(string id) =>
            this.players.Find<Player>(player => player.Id == id).FirstOrDefault();

        /// <summary>
        /// Get all players from a specified team.
        /// </summary>
        /// <param name="teamId"></param>
        /// <returns></returns>
        public async Task<List<Player>> GetFromTeamAsync(string teamId)
        {
            return await this.players.Find(player => player.TeamId == teamId).ToListAsync();
        }

        public Player Create(Player player)
        {
            this.players.InsertOne(player);
            return player;
        }

        public void Update(string id, Player playerIn) =>
            this.players.ReplaceOne(player => player.Id == id, playerIn);

        public void Remove(Player playerIn) =>
            this.players.DeleteOne(player => player.Id == playerIn.Id);

        public void Remove(string id) =>
            this.players.DeleteOne(player => player.Id == id);
    }
}
