using COAL.CORE.Core.Game;
using COAL.CORE.Models;
using COAL.PES.Data;
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
        private readonly ClubService clubService;
        private readonly TeamService teamService;
        private readonly MatchService matchService;
        private readonly IMongoDatabase mainDatabase;
        private readonly PlayerService playerService;

        public SaveGameService(ICoalDatabaseSettings settings,
            GeneratorService generatorService,
            TableService tableService,
            CompetitionService competitionService,
            ClubService clubService,
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
            this.clubService = clubService;
            this.teamService = teamService;
            this.matchService = matchService;
            this.playerService = playerService;
        }

        /// <summary>
        /// Loads a save game and initializes the DB connection.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="init">Init everything, if it´s the first time.</param>
        public async Task<bool> LoadAsync(SaveGame saveGame, bool init = false)
        {
            this.settings.DatabaseName = saveGame.SaveGameId;
            this.settings.DataPath = saveGame.DataPath;
            this.RefreshServices();

            // If database doesn´t exist yet, create it.
            MongoClient client = new MongoClient(settings.ConnectionString);
            var db = client.GetDatabase(saveGame.SaveGameId);

            //IMongoCollection<object> playersCollection = db.GetCollection<object>("Players");

            //if(playersCollection == null)
            //{
            //    // Initialize all collections.
            //    var taskPlayers = db.CreateCollectionAsync("Players");
            //    var taskTeams = db.CreateCollectionAsync("Teams");
            //    var taskCoaches = db.CreateCollectionAsync("Coaches");
            //    var taskContracts = db.CreateCollectionAsync("Contracts");

            //    await Task.WhenAll(taskPlayers, taskTeams, taskCoaches, taskContracts);
            //}

            if (init)
            {
                // Step 1: Read PES data and transform it to COAL data
                PESDataReader reader = new PESDataReader();

                // Get Clubs and insert them into COAL DB.
                var clubs = await reader.ReadClubsAsync(settings.DataPath);
                clubs = await this.clubService.CreateManyAsync(clubs);

                // Generate teams.
                var teams = await this.generatorService.GenerateTeamsAsync(clubs);

                // Get Players and insert them into COAL DB.
                var players = await reader.ReadPlayersAsync(settings.DataPath);
                players = await this.playerService.CreateManyAsync(players);

                // ToDo: Get competitions (competitions, competition regulations, )

                // Get Competitions
                var competitions = await reader.ReadCompetitionsAsync(settings.DataPath);
                competitions = await this.competitionService.CreateManyAsync(competitions);

                // Apply competition entries ( competition <-> team assignments)
                var competitionEntries = await reader.ReadCompetitionEntriesAsync(settings.DataPath);
                competitions = await this.competitionService.ApplyCompetitionEntriesAsync(competitionEntries);

                // Create tables and matches for competitions
                foreach (var competition in competitions)
                {
                    if(competition.Teams.Count == 0)
                    {
                        continue;
                    }

                    await this.tableService.CreateFromCompetitionAsync(competition);

                    if (competition.Teams.Count > 2)
                    {
                        await this.matchService.CreateFixturesFromCompetitionAsync(competition);
                    }
                }
                

                // Step 2: Apply data assignments (player <-> team, etc.)
                var teamAssignments = await reader.ReadPlayerAssignmentsAsync(settings.DataPath);
                await this.playerService.ApplyTeamAssignments(teamAssignments);

                // Step 3: Generate additional data we need ( reserve players, youth players, coaches, contracts, etc.)
                var additionalPlayers = await this.generatorService.GeneratePlayersAsync(clubs, ClubGenerationMode.ReservesAndYouth);

                // Step 4: Generate lower leagues completely randomly
                
                // ToDo: Get real contract lenght (and maybe wages)
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
            await LoadAsync(saveGame, true).ConfigureAwait(false);
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
            this.clubService.Refresh();
            this.teamService.Refresh();
            this.matchService.Refresh();
            this.playerService.Refresh();
        }
    }
}
