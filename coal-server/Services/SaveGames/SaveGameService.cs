using COAL.CORE.Core.Game;
using COAL.CORE.Models;
using COAL.PES.Data;
using CoalServer.Services.Clubs;
using CoalServer.Services.Competitions;
using CoalServer.Services.Generator;
using CoalServer.Services.Matches;
using CoalServer.Services.Players;
using CoalServer.Services.Tables;
using CoalServer.Services.Teams;
using MongoDB.Driver;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoalServer.Services.SaveGames
{
    public class SaveGameService : ISaveGameService
    {
        private IMongoCollection<SaveGame> saveGames;
        private readonly ICoalDatabaseSettings settings;
        private readonly IGeneratorService generatorService;
        private readonly ITableService tableService;
        private readonly ICompetitionService competitionService;
        private readonly IClubService clubService;
        private readonly ITeamService teamService;
        private readonly IMatchService matchService;
        private readonly IMongoDatabase mainDatabase;
        private readonly IPlayerService playerService;

        public SaveGameService(ICoalDatabaseSettings settings,
            IGeneratorService generatorService,
            ITableService tableService,
            ICompetitionService competitionService,
            IClubService clubService,
            ITeamService teamService,
            IMatchService matchService,
            IPlayerService playerService)
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
        public async Task LoadAsync(SaveGame saveGame, bool init = false)
        {
            this.settings.DatabaseName = saveGame.SaveGameId;
            this.settings.DataPath = saveGame.DataPath;
            this.Refresh();

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
                await this.InitAsync();
            }
        }

        /// <summary>
        /// Moves forward one day and simulates everything.
        /// </summary>
        /// <returns></returns>
        public async Task<SaveGame> IncrementIngameDateAsync()
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
            await this.UpdateAsync(activeSaveGame.Id, activeSaveGame);
            return activeSaveGame;
        }

        public async Task<SaveGame> GetByNameAsync(string saveGameId)
        {
            return await this.saveGames.Find<SaveGame>(saveGame => saveGame.SaveGameId == saveGameId).FirstOrDefaultAsync();
        }    

        /// <summary>
        /// Returns all saveGames.
        /// </summary>
        /// <returns></returns>
        public async Task<List<SaveGame>> GetAsync()
        {
            return await this.saveGames.FindAsync(player => true).Result.ToListAsync();
        }

        /// <summary>
        /// Returns a saveGame with a specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<SaveGame> GetAsync(string id)
        {
            return await this.saveGames.FindAsync<SaveGame>(saveGame => saveGame.Id == id).Result.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new saveGame entity.
        /// </summary>
        /// <param name="saveGame"></param>
        /// <returns></returns>
        public async Task<SaveGame> CreateAsync(SaveGame saveGame)
        {
            await this.saveGames.InsertOneAsync(saveGame);
            await LoadAsync(saveGame, true).ConfigureAwait(false);
            return saveGame;
        }

        /// <summary>
        /// Insert a list of saveGames.
        /// </summary>
        /// <param name="saveGames"></param>
        /// <returns></returns>
        public async Task<List<SaveGame>> CreateManyAsync(List<SaveGame> saveGames)
        {
            await this.saveGames.InsertManyAsync(saveGames);

            return saveGames;
        }

        /// <summary>
        /// Updates a specific saveGame entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="saveGameIn"></param>
        /// <returns></returns>
        public async Task UpdateAsync(string id, SaveGame saveGameIn)
        {
            await this.saveGames.ReplaceOneAsync(saveGame => saveGame.Id == id, saveGameIn);
        }

        /// <summary>
        /// Updates a list of saveGames
        /// </summary>
        /// <param name="saveGamesIn"></param>
        /// <returns></returns>
        public async Task UpdateManyAsync(List<SaveGame> saveGamesIn)
        {
            saveGamesIn.ForEach(async p => { await this.UpdateAsync(p.Id, p); });
        }

        /// <summary>
        /// Removes a saveGame
        /// </summary>
        /// <param name="saveGameIn"></param>
        /// <returns></returns>
        public async Task RemoveAsync(SaveGame saveGameIn)
        {
            await this.saveGames.DeleteOneAsync(saveGame => saveGame.Id == saveGameIn.Id);
        }


        /// <summary>
        /// Remove the saveGame with an specific id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveAsync(string id)
        {
            await this.saveGames.DeleteOneAsync(saveGame => saveGame.Id == id);
        }


        /// <summary>
        /// Initialize eco-system for the first time.
        /// </summary>
        /// <returns></returns>
        private async Task InitAsync()
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
                if (competition.Teams.Count == 0)
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
            await this.playerService.ApplyTeamAssignmentsAsync(teamAssignments);

            // Step 3: Generate additional data we need ( reserve players, youth players, coaches, contracts, etc.)
            var additionalPlayers = await this.generatorService.GeneratePlayersAsync(clubs, ClubGenerationMode.ReservesAndYouth);

            // Step 4: Generate lower leagues completely randomly

            // ToDo: Get real contract lenght (and maybe wages)
            var contracts = await this.generatorService.GenerateContractsAsync(players);
        }

        /// <summary>
        /// Refreshes all services.
        /// </summary>
        public void Refresh()
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
