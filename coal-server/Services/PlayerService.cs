using COAL.CORE.Models;
using COAL.CORE.Models.Team;
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
        private IMongoCollection<Club> clubs;
        private readonly ICoalDatabaseSettings settings;

        public PlayerService(ICoalDatabaseSettings settings)
        {
            this.ConnectToDb(settings);
            this.settings = settings;
        }

        /// <summary>
        /// Connects to the currently loaded DB.
        /// </summary>
        /// <param name="settings"></param>
        private void ConnectToDb(ICoalDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            IMongoDatabase database = client.GetDatabase(settings.DatabaseName);

            this.players = database.GetCollection<Player>(settings.PlayersCollectionName);
            this.clubs = database.GetCollection<Club>(settings.ClubsCollectionName);
        }

        /// <summary>
        /// Refreshes database connection.
        /// </summary>
        public void Refresh()
        {
            this.ConnectToDb(settings);
        }

        /// <summary>
        /// Applys a list of team assignments. Sets player´s club and team ids according to the assignments.
        /// </summary>
        /// <param name="assignments"></param>
        /// <returns></returns>
        public async Task<bool> ApplyTeamAssignments(List<TeamAssignment> assignments)
        {
            List<Player> tempList = new List<Player>();

            assignments.ForEach(a =>
            {
                // get club
                var club = this.clubs.Find(t => t.SourceId == a.TeamId).FirstOrDefault();

                // get player
                Player temp = this.players.Find(p => p.SourceId == a.PlayerId).FirstOrDefault();

                // assign
                temp.ClubId = club.ClubId;
                temp.TeamId = club.ClubId + "T1";
                temp.ShirtNumber = a.ShirtNumber;

                tempList.Add(temp);
            });

            this.UpdateMany(tempList);

            return await Task.FromResult(true);
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

        /// <summary>
        /// Create a player.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Player Create(Player player)
        {
            this.players.InsertOne(player);
            return player;
        }

        /// <summary>
        /// Insert a list of players.
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public async Task<List<Player>> CreateManyAsync(List<Player> players)
        {
            await this.players.InsertManyAsync(players);

            return players;
        }

        public async Task UpdateAsync(string id, Player playerIn) =>
            await this.players.ReplaceOneAsync(player => player.Id == id, playerIn);

        /// <summary>
        /// Updates a list of players
        /// </summary>
        /// <param name="playersIn"></param>
        /// <returns></returns>
        public void UpdateMany(List<Player> playersIn)
        {
            playersIn.ForEach(async p => { await this.UpdateAsync(p.Id, p); });
        }

        public void Remove(Player playerIn) =>
            this.players.DeleteOne(player => player.Id == playerIn.Id);

        public void Remove(string id) =>
            this.players.DeleteOne(player => player.Id == id);
    }
}
