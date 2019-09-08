using COAL.CORE.Core.Game;
using COAL.CORE.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoalServer.Services
{
    public class SaveGameService
    {
        private IMongoCollection<SaveGame> saveGames;
        private readonly ICoalDatabaseSettings settings;
        private readonly GeneratorService generatorService;
        private readonly TableService tableService;
        private readonly CompetitionService competitionService;
        private readonly TeamService teamService;
        private readonly MatchService matchService;
        private readonly IMongoDatabase mainDatabase;
        private readonly PlayerService playerService;

        public SaveGameService(ICoalDatabaseSettings settings,
            GeneratorService generatorService,
            TableService tableService,
            CompetitionService competitionService,
            TeamService teamService,
            MatchService matchService,
            PlayerService playerService)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            this.mainDatabase = client.GetDatabase(settings.MainDatabase);

            this.saveGames = this.mainDatabase.GetCollection<SaveGame>(settings.SaveGamesCollectionName);
            this.settings = settings;
            this.generatorService = generatorService;
            this.tableService = tableService;
            this.competitionService = competitionService;
            this.teamService = teamService;
            this.matchService = matchService;
            this.playerService = playerService;
        }

        /// <summary>
        /// Loads a save game and initializes the DB connection.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="init">Init everything, if it´s the first time.</param>
        public async Task<bool> LoadAsync(string id, bool init = false)
        {
            this.settings.DatabaseName = id;
            this.RefreshServices();

            // If database doesn´t exist yet, create it.
            MongoClient client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(id);

            IMongoCollection<object> playersCollection = db.GetCollection<object>("Players");

            if(playersCollection == null)
            {
                // Initialize all collections.
                var taskPlayers = db.CreateCollectionAsync("Players");
                var taskTeams = db.CreateCollectionAsync("Teams");
                var taskCoaches = db.CreateCollectionAsync("Coaches");
                var taskContracts = db.CreateCollectionAsync("Contracts");

                await Task.WhenAll(taskPlayers, taskTeams, taskCoaches, taskContracts);
            }

            if (init)
            {
                this.generatorService.Refresh();

                var clubs = await this.generatorService.GenerateClubsAsync();
                var teams = await this.generatorService.GenerateTeamsAsync(clubs);
                var competitions = await this.generatorService.GenerateCompetitionsAsync();
                var players = await this.generatorService.GeneratePlayersAsync(clubs);
                var contracts = await this.generatorService.GenerateContractsAsync(players);

                return true;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Moves forward one day and simulates everything.
        /// </summary>
        /// <returns></returns>
        public async Task<SaveGame> IncrementIngameDate()
        {
            SaveGame activeSaveGame = await this.saveGames.Find(s => s.SaveGameId == settings.DatabaseName).FirstOrDefaultAsync();
            activeSaveGame.InGameDate = activeSaveGame.InGameDate.AddDays(1);

            var matches = await this.matchService.GetMatchesToSimulateAsync(activeSaveGame.InGameDate);
            matches = await this.matchService.SimulateMatchesAsync(matches);

            // Apply matches to corresponding tables.
            await this.tableService.ApplyMatchesAsync(matches);

            // all competitions -> all matches for today
            // -> Each match -> adjusts tables, player bios, player injuries, player mood, player development
            // Create news entries for big matches

            // potential outrunning contracts 

            // club / team budgets

            // coaches deciding on transfer offers / requests
            // coaches deciding on transfer goals.
            // coaches making transfer offers / requests / etc.



            // ToDo: GameLogic service -> Simulate and update everything
            this.Update(activeSaveGame.Id, activeSaveGame);
            return activeSaveGame;
        }

        public List<SaveGame> Get() =>
            this.saveGames.Find(saveGame => true).ToList();

        public SaveGame Get(string id) =>
            this.saveGames.Find<SaveGame>(saveGame => saveGame.Id == id).FirstOrDefault();

        public SaveGame GetByName(string saveGameId) =>
            this.saveGames.Find<SaveGame>(saveGame => saveGame.SaveGameId == saveGameId).FirstOrDefault();

        /// <summary>
        ///  Create a new Save Game.
        ///  Start initializing the data import.
        /// </summary>
        /// <param name="saveGame"></param>
        /// <returns></returns>
        public async Task<SaveGame> CreateAsync(SaveGame saveGame)
        {
            this.saveGames.InsertOne(saveGame);
            await LoadAsync(saveGame.SaveGameId, true).ConfigureAwait(false);
            return saveGame;
        }

        public void Update(string id, SaveGame saveGameIn) =>
            this.saveGames.ReplaceOne(saveGame => saveGame.Id == id, saveGameIn);

        public void Remove(SaveGame saveGameIn) =>
            this.saveGames.DeleteOne(saveGame => saveGame.Id == saveGameIn.Id);

        public void Remove(string id) =>
            this.saveGames.DeleteOne(saveGame => saveGame.Id == id);

        /// <summary>
        /// Refreshes all services.
        /// </summary>
        private void RefreshServices()
        {
            this.generatorService.Refresh();
            this.competitionService.Refresh();
            this.tableService.Refresh();
            this.teamService.Refresh();
            this.matchService.Refresh();
            this.playerService.Refresh();
        }
    }
}
