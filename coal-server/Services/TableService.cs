using COAL.CORE.Models;
using COAL.CORE.Models.Competition;
using COAL.CORE.Models.Competition.Matches;
using COAL.CORE.Models.Team;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoalServer.Services
{
    public class TableService
    {
        private IMongoDatabase database;

        private readonly ICoalDatabaseSettings settings;
        private IMongoCollection<Table> tables;
        private IMongoCollection<Team> teams;

        public TableService(ICoalDatabaseSettings settings)
        {
            this.ConnectToDb(settings);
            this.settings = settings;
        }

        private void ConnectToDb(ICoalDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            this.database = client.GetDatabase(settings.DatabaseName);

            this.tables = this.database.GetCollection<Table>(settings.TablesCollectionName);
            this.teams = this.database.GetCollection<Team>(settings.TeamsCollectionName);
        }

        /// <summary>
        /// Refresh the database connection of the service.
        /// </summary>
        public void Refresh()
        {
            this.ConnectToDb(settings);
        }

        public List<Table> Get() =>
            this.tables.Find(table => true).ToList();

        public Table Get(string id) =>
            this.tables.Find<Table>(table => table.Id == id).FirstOrDefault();

        public Table GetFromCompetitonId(string competitionId) =>
            this.tables.Find<Table>(table => table.CompetitionId == competitionId).FirstOrDefault();

        public Table Create(Table table)
        {
            this.tables.InsertOne(table);
            return table;
        }

        public void Update(string id, Table tableIn) =>
            this.tables.ReplaceOne(table => table.Id == id, tableIn);

        public void Remove(Table tableIn) =>
            this.tables.DeleteOne(table => table.Id == tableIn.Id);

        public void Remove(string id) =>
            this.tables.DeleteOne(table => table.Id == id);

        /// <summary>
        /// Applies a list of matches to a table.
        /// </summary>
        /// <param name="matches"></param>
        /// <returns></returns>
        public async Task<List<Table>> ApplyMatchesAsync(List<Match> matches)
        {
            List<Table> affectedTables = new List<Table>();

            matches.GroupBy(m => m.TableId).ToList().ForEach(async matchList =>
            {
                string tableId = matchList.Key;
                Table table = this.tables.Find(t => t.TableId == tableId).First();

                matchList.ToList().ForEach(m =>
                {
                    TableEntry homeTeam = table.Teams.FirstOrDefault(t => t.TeamId == m.HomeTeamId);
                    TableEntry awayTeam = table.Teams.FirstOrDefault(t => t.TeamId == m.AwayTeamId);

                    homeTeam.GamesPlayed++;
                    awayTeam.GamesPlayed++;
                    homeTeam.GoalsScored += m.HomeGoals;
                    homeTeam.GoalsAllowed += m.AwayGoals;
                    awayTeam.GoalsScored += m.AwayGoals;
                    awayTeam.GoalsAllowed += m.HomeGoals;

                    if (m.HomeGoals > m.AwayGoals)
                    {
                        homeTeam.GamesWon++;
                        awayTeam.GamesLost++;
                    }
                    else if (m.HomeGoals == m.AwayGoals)
                    {
                        homeTeam.GamesDrawn++;
                        awayTeam.GamesDrawn++;
                    }
                    else
                    {
                        awayTeam.GamesWon++;
                        homeTeam.GamesLost++;
                    }
                });

                table.Teams.Sort(delegate (TableEntry x, TableEntry y)
                {
                    if ((x.GamesWon * 3 + x.GamesDrawn) > (y.GamesWon * 3 + y.GamesDrawn)) { return -1; }
                    else if ((x.GamesWon * 3 + x.GamesDrawn) < (y.GamesWon * 3 + y.GamesDrawn)) { return 1; }
                    else
                    {
                        return (x.GoalsScored - x.GoalsAllowed) > (y.GoalsScored - y.GoalsAllowed) ? -1 : 1;
                    }
                });

                await this.tables.ReplaceOneAsync(t => t.Id == table.Id, table);
                affectedTables.Add(table);
            });

            return await Task.FromResult(affectedTables);
        }

        /// <summary>
        /// Creates a table to a competition.
        /// </summary>
        /// <param name="competition"></param>
        /// <returns></returns>
        public async Task<Table> CreateFromCompetitionAsync(Competition competition)
        {
            Table table = new Table()
            {
                TableId = competition.CompetitionId + "-T" + competition.Season,
                CompetitionId = competition.CompetitionId
            };

            competition.Teams.ForEach(t => {
                table.Teams.Add(new TableEntry()
                {
                    TableId = table.TableId,
                    TeamId = t,
                    TeamName = this.teams.Find(x => x.TeamId == t).FirstOrDefault().Name,
                });
            });

            await this.tables.InsertOneAsync(table);
            return table;
        }
    }
}
