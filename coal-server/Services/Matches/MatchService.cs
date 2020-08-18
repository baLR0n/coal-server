using COAL.CORE.Models;
using COAL.CORE.Models.Competition;
using COAL.CORE.Models.Competition.Matches;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoalServer.Services.Matches
{
    public class MatchService : IMatchService
    {
        private IMongoDatabase database;

        private readonly ICoalDatabaseSettings settings;
        private IMongoCollection<Match> matches;
        private IMongoCollection<Table> tables;

        public MatchService(ICoalDatabaseSettings settings)
        {
            this.ConnectToDb(settings);
            this.settings = settings;
        }

        private void ConnectToDb(ICoalDatabaseSettings settings)
        {
            MongoClient client = new MongoClient(settings.ConnectionString);
            this.database = client.GetDatabase(settings.DatabaseName);

            this.matches = this.database.GetCollection<Match>(settings.MatchesCollectionName);
            this.tables = this.database.GetCollection<Table>(settings.TablesCollectionName);
        }

        /// <summary>
        /// Refresh the database connection of the service.
        /// </summary>
        public void Refresh()
        {
            this.ConnectToDb(settings);
        }

        public async Task<List<Match>> GetFixturesForTableAsync(string tableId)
        {
            return await this.matches.Find(m => m.TableId == tableId).ToListAsync();
        }

        /// <summary>
        /// Creates a match to a competition.
        /// </summary>
        /// <param name="competition"></param>
        /// <returns></returns>
        public async Task<List<Match>> CreateFixturesFromCompetitionAsync(Competition competition)
        {
            DateTime startDate = competition.StartDate;

            // We want the first game on a friday.
            // ToDo: Last years champion should be in the first game.
            while (startDate.DayOfWeek != DayOfWeek.Friday)
            {
                startDate = startDate.AddDays(1);
            }

            Table table = await this.tables.Find(t => t.CompetitionId == competition.CompetitionId).FirstOrDefaultAsync();
            List<Match> fixtures = this.RoundRobinMatches(competition.Teams, startDate, table.TableId);

            await this.matches.InsertManyAsync(fixtures);
            return fixtures;
        }

        /// <summary>
        /// Starts a match simulation for a list of matches.
        /// </summary>
        /// <param name="matches"></param>
        /// <returns></returns>
        public async Task<List<Match>> SimulateMatchesAsync(List<Match> matches)
        {
            // ToDo: Obviously, this is just a test implementation.
            // ToDo: This will be the real meat. Add a chance for a GameEvent every minute and
            //       continue from there on with a realistic game procession.
            Random rng = new Random();
            matches.ForEach(async m =>
            {
                var rngHome = rng.Next(0, 9);
                if (rngHome < 7)
                {
                    m.HomeGoals = rng.Next(0, 3);
                }
                else if (rngHome < 9)
                {
                    m.HomeGoals = rng.Next(0, 5);
                }
                else
                {
                    m.HomeGoals = rng.Next(0, 6);
                }

                var rngAway = rng.Next(0, 9);
                if (rngAway < 7)
                {
                    m.AwayGoals = rng.Next(0, 3);
                }
                else if (rngAway < 9)
                {
                    m.AwayGoals = rng.Next(0, 5);
                }
                else
                {
                    m.AwayGoals = rng.Next(0, 6);
                }

                await this.UpdateAsync(m.Id, m);
            });

            return matches;
        }

        /// <summary>
        /// Gets all matches that are due to a specified date and time.
        /// </summary>
        /// <param name="inGameDate"></param>
        /// <returns></returns>
        public async Task<List<Match>> GetMatchesToSimulateAsync(DateTime inGameDate)
        {
            DateTime currentDayStart = inGameDate.Date;
            DateTime currentDayEnds = inGameDate.Date.AddDays(1);
            return await this.matches.Find(m => m.DateTime >= currentDayStart && m.DateTime < currentDayEnds).ToListAsync();
        }

        /// <summary>
        /// Returns all matches.
        /// </summary>
        /// <returns></returns>
        public async Task<List<Match>> GetAsync()
        {
            return await this.matches.FindAsync(player => true).Result.ToListAsync();
        }

        /// <summary>
        /// Returns a match with a specified ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<Match> GetAsync(string id)
        {
            return await this.matches.FindAsync<Match>(match => match.Id == id).Result.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a new match entity.
        /// </summary>
        /// <param name="match"></param>
        /// <returns></returns>
        public async Task<Match> CreateAsync(Match match)
        {
            await this.matches.InsertOneAsync(match);
            return match;
        }

        /// <summary>
        /// Insert a list of matches.
        /// </summary>
        /// <param name="matches"></param>
        /// <returns></returns>
        public async Task<List<Match>> CreateManyAsync(List<Match> matches)
        {
            await this.matches.InsertManyAsync(matches);

            return matches;
        }

        /// <summary>
        /// Updates a specific match entity
        /// </summary>
        /// <param name="id"></param>
        /// <param name="matchIn"></param>
        /// <returns></returns>
        public async Task UpdateAsync(string id, Match matchIn)
        {
            await this.matches.ReplaceOneAsync(match => match.Id == id, matchIn);
        }

        /// <summary>
        /// Updates a list of matches
        /// </summary>
        /// <param name="matchesIn"></param>
        /// <returns></returns>
        public async Task UpdateManyAsync(List<Match> matchesIn)
        {
            matchesIn.ForEach(async p => { await this.UpdateAsync(p.Id, p); });
        }

        /// <summary>
        /// Removes a match
        /// </summary>
        /// <param name="matchIn"></param>
        /// <returns></returns>
        public async Task RemoveAsync(Match matchIn)
        {
            await this.matches.DeleteOneAsync(match => match.Id == matchIn.Id);
        }


        /// <summary>
        /// Remove the match with an specific id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task RemoveAsync(string id)
        {
            await this.matches.DeleteOneAsync(match => match.Id == id);
        }

        /// <summary>
        /// Creates a fixture schedule with the round robin algorithm.
        /// </summary>
        /// <param name="teamList"></param>
        /// <param name="startDate"></param>
        /// <param name="tableId"></param>
        /// <returns></returns>
        private List<Match> RoundRobinMatches(List<string> teamList, DateTime startDate, string tableId)
        {
            List<Match> fixtures = new List<Match>();
            int teamCount = teamList.Count;
            Random r = new Random();

            if (teamList.Count % 2 != 0)
            {
                teamList.Add("Bye");
            }

            int numDays = (teamCount - 1);
            int halfSize = teamCount / 2;

            List<string> teams = new List<string>();

            teams.AddRange(teamList.Skip(halfSize).Take(halfSize));
            teams.AddRange(teamList.Skip(1).Take(halfSize - 1).ToArray().Reverse());
            int teamsSize = teams.Count;

            for (int day = 0; day < numDays; day++)
            {
                List<Match> gameDayMatches = new List<Match>();

                int teamIdx = day % teamsSize;

                // Every other gameday is an away game
                bool evenDay = day % 2 == 0;

                gameDayMatches.Add(new Match
                {
                    HomeTeamId = evenDay ? teams[teamIdx] : teamList[0],
                    AwayTeamId = evenDay ? teamList[0] : teams[teamIdx],
                    GameDay = day + 1,
                    TableId = tableId
                });

                for (int idx = 1; idx < halfSize; idx++)
                {
                    int firstTeam = evenDay ? (day + idx) % teamsSize : (day + teamsSize - idx) % teamsSize;
                    int secondTeam = evenDay ? (day + teamsSize - idx) % teamsSize : (day + idx) % teamsSize;
                    gameDayMatches.Add(new Match
                    {
                        HomeTeamId = teams[firstTeam],
                        AwayTeamId = teams[secondTeam],
                        GameDay = day + 1,
                        TableId = tableId
                    });
                }

                // Apply differente weekdays to the matches
                // ToDo: Settings for each competition about how much games on each weekday.
                startDate = this.getNextDayOfWeek(startDate, DayOfWeek.Friday);
                gameDayMatches = this.Shuffle<Match>(gameDayMatches);

                // 1 Friday game
                gameDayMatches[0].DateTime = startDate;

                // Saturday games
                for (int i = 1; i < gameDayMatches.Count - 2; i++)
                {
                    gameDayMatches[i].DateTime = this.getNextDayOfWeek(startDate, DayOfWeek.Saturday);
                }

                // 2 Sunday games
                gameDayMatches[gameDayMatches.Count - 2].DateTime = this.getNextDayOfWeek(startDate, DayOfWeek.Sunday);
                gameDayMatches[gameDayMatches.Count - 1].DateTime = this.getNextDayOfWeek(startDate, DayOfWeek.Sunday);

                fixtures.AddRange(gameDayMatches);
                startDate = this.getNextDayOfWeek(startDate, DayOfWeek.Monday);
            }

            return fixtures;
        }


        // ToDo: Util service

        /// <summary>
        /// Gets the next specified day of week from a starting date.
        /// </summary>
        /// <param name="date"></param>
        /// <param name="dayOfWeek"></param>
        /// <returns></returns>
        private DateTime getNextDayOfWeek(DateTime date, DayOfWeek dayOfWeek)
        {
            while(date.DayOfWeek != dayOfWeek)
            {
                date = date.AddDays(1);
            }

            return date;
        }

        /// <summary>
        /// Randomly shuffles a list of objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        private List<T> Shuffle<T>(List<T> list)
        {
            Random rng = new Random();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }
    }
}
