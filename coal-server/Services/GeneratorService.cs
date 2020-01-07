using COAL.CORE.Models;
using COAL.CORE.Models.Competition;
using COAL.CORE.Models.Team;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CoalServer.Services
{
    public class GeneratorService
    {
        private IMongoDatabase database;

        private IMongoCollection<Competition> competitions;
        private IMongoCollection<Player> players;
        private IMongoCollection<Club> clubs;
        private IMongoCollection<Team> teams;
        private IMongoCollection<Contract> contracts;

        private readonly ICoalDatabaseSettings settings;
        private readonly TableService tableService;
        private readonly MatchService matchService;

        /// <summary>
        /// Initializes a new instance of GeneratorService
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="tableService"></param>
        public GeneratorService(ICoalDatabaseSettings settings, TableService tableService, MatchService matchService)
        {
            this.ConnectToDb(settings);
            this.settings = settings;
            this.tableService = tableService;
            this.matchService = matchService;
        }

        /// <summary>
        /// Establishes a connection to the database
        /// </summary>
        /// <param name="settings"></param>
        private void ConnectToDb(ICoalDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            this.database = client.GetDatabase(settings.DatabaseName);

            this.competitions = database.GetCollection<Competition>(settings.CompetitionsCollectionName);
            this.players = database.GetCollection<Player>(settings.PlayersCollectionName);
            this.clubs = database.GetCollection<Club>(settings.ClubsCollectionName);
            this.teams = database.GetCollection<Team>(settings.TeamsCollectionName);
            this.contracts = database.GetCollection<Contract>(settings.ContractsCollectionName);
        }

        /// <summary>
        /// Refresh the database connection of the service.
        /// </summary>
        public void Refresh()
        {
            this.ConnectToDb(settings);
            this.tableService.Refresh();
            this.matchService.Refresh();
        }

        /// <summary>
        /// Generates competitions and corresponding tables
        /// </summary>
        /// <returns></returns>
        public async Task<List<Competition>> GenerateCompetitionsAsync()
        {
            List<Competition> results = new List<Competition>();
            Random r = new Random();

            var clubs = this.database.GetCollection<Club>(settings.ClubsCollectionName);
            var clubList = await clubs.Find(x => true).ToListAsync();

            for (int i = 0; i < 10; i++)
            {
                Competition c = new Competition()
                {
                    CompetitionId = Guid.NewGuid().ToString(),
                    Name = (i + 1) + ". Liga ",
                    Season = 19,
                    Teams = new List<string>(),
                    Level = i + 1,
                    StartDate = DateTime.Today
                };

                for (int j = i * 18; j < (i * 18) + 18; j++)
                {
                    c.Teams.Add(clubList[j].FirstTeamId);
                }

                await this.tableService.CreateFromCompetitionAsync(c);
                await this.matchService.CreateFixturesFromCompetitionAsync(c);

                results.Add(c);
            }

            await this.competitions.InsertManyAsync(results);
            return results;
        }

        /// <summary>
        /// Generates clubs 
        /// </summary>
        /// <returns></returns>
        public async Task<List<Club>> GenerateClubsAsync()
        {
            List<Club> results = new List<Club>();
            Random r = new Random();

            for (int i = 0; i < 1000; i++)
            {
                string clubGuid = Guid.NewGuid().ToString();
                Club club = new Club()
                {
                    ClubId = clubGuid,
                    FirstTeamId = clubGuid + "T1",
                    ReservesTeamId = clubGuid + "T2",
                    YouthTeamId = clubGuid + "T3",
                    Name = "Club " + i,
                    CountryId = r.Next(0, 10),
                    DomesticPrestige = r.Next(1, 10),
                    InternationalPrestige = r.Next(1, 10),
                };

                results.Add(club);
            }

            await this.clubs.InsertManyAsync(results);
            return results;
        }

        /// <summary>
        /// Generates teams
        /// </summary>
        /// <param name="clubs"></param>
        /// <returns></returns>
        public async Task<List<Team>> GenerateTeamsAsync(List<Club> clubs, ClubGenerationMode mode = ClubGenerationMode.AllTeams)
        {
            List<Team> results = new List<Team>();

            for (int i = 0; i < clubs.Count; i++)
            {
                Club club = clubs[i];

                if ((int)mode <= 3)
                {

                    // First team
                    Team firstTeam = new Team()
                    {
                        TeamId = club.FirstTeamId,
                        ClubId = club.ClubId,
                        Name = club.Name,
                    };

                    results.Add(firstTeam);
                }

                // Second team
                Team secondTeam = new Team()
                {
                    TeamId = club.ReservesTeamId,
                    ClubId = club.ClubId,
                    Name = club.Name + " II",
                };

                // Youth team
                Team youthTeam = new Team()
                {
                    TeamId = club.YouthTeamId,
                    ClubId = club.ClubId,
                    Name = club.Name + " U19",
                };

                
                results.Add(secondTeam);
                results.Add(youthTeam);
            }

            await this.teams.InsertManyAsync(results);
            return results;
        }


        /// <summary>
        /// Generate players into the current database.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="countryId"></param>
        /// <returns></returns>
        public async Task<List<Player>> GeneratePlayersAsync(List<Club> clubs, ClubGenerationMode mode = ClubGenerationMode.AllTeams)
        {
            List<Player> results = new List<Player>();
            Random r = new Random();

            for (int i = 0; i < clubs.Count; i++)
            {
                Club club = clubs[i];

                // ToDo: Handle all generation modes.
                if ((int)mode <= 3)
                {
                    // First team
                    for (int j = 0; j < r.Next(20, 28); j++)
                    {
                        Player player = new Player()
                        {
                            PlayerId = Guid.NewGuid().ToString(),
                            ClubId = club.ClubId,
                            TeamId = club.FirstTeamId,
                            Name = "M. Lohr " + i,
                            ShirtName = "Lohr" + i,
                            Overall = r.Next(40, 99),
                        };
                        results.Add(player);
                    }
                }

                // Second team
                for (int j = 0; j < r.Next(20, 28); j++)
                {
                    Player player = new Player()
                    {
                        PlayerId = Guid.NewGuid().ToString(),
                        ClubId = club.Id,
                        TeamId = club.ReservesTeamId,
                        Name = "M. Lohr " + i,
                        ShirtName = "Lohr" + i,
                        Overall = r.Next(40, 99),
                    };
                    results.Add(player);
                }

                // Youth team
                for (int j = 0; j < r.Next(20, 28); j++)
                {
                    Player player = new Player()
                    {
                        PlayerId = Guid.NewGuid().ToString(),
                        ClubId = club.Id,
                        TeamId = club.YouthTeamId,
                        Name = "M. Lohr " + i,
                        ShirtName = "Lohr" + i,
                        Overall = r.Next(40, 99),
                    };
                    results.Add(player);
                }
            }

            await this.players.InsertManyAsync(results);
            return results;
        }

        /// <summary>
        /// Generates a contract for a set of players
        /// </summary>
        /// <param name="players"></param>
        /// <returns></returns>
        public async Task<List<Contract>> GenerateContractsAsync(List<Player> players)
        {
            List<Contract> results = new List<Contract>();
            Random r = new Random();
            DateTime contractDate = new DateTime(2019, 7, 1);

            for (int i = 0; i < players.Count; i++)
            {
                Player player = players[i];
                Contract contract = new Contract()
                {
                    ContractId = player.PlayerId + "C1",
                    PlayerId = player.PlayerId,
                    ClubId = player.ClubId,
                    EndDate = contractDate.AddYears(r.Next(1, 5)),
                    ReleaseClause = r.Next(0, 5) == 0 ? r.Next(100000, 10000000) : 0,
                    WeeklyWage = r.Next(720, 300000)
                };

                results.Add(contract);
            }

            await this.contracts.InsertManyAsync(results);
            return results;
        }
    }

    public enum ClubGenerationMode
    {
        AllTeams,
        FirstOnly,
        FirstAndReserves,
        FirstAndYouth,
        ReservesAndYouth,
        ReservesOnly,
        YouthOnly
    }
}
