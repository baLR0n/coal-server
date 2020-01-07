using COAL.CORE.Models;
using COAL.CORE.Models.Competition;
using COAL.CORE.Models.Team;
using COAL.PES.Data;
using COAL.PES.Models;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    public class PESDataReaderTests
    {
        private PESDataReader reader;

        [SetUp]
        public void Setup()
        {
            this.reader = new PESDataReader();
        }

        /// <summary>
        /// Tests the extracting of player data from the PES binary files.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestReadPlayersAsync()
        {
            List<Player> result = await this.reader.ReadPlayersAsync("C:\\pesdb");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        /// <summary>
        /// Tests the importing of player data into the PES binary files.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestUpdatePlayerAsync()
        {
            List<Player> result = await this.reader.ReadPlayersAsync("C:\\pesdb");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            Player p = result[0];
            string newName = "Random Name " + DateTime.Now.Millisecond.ToString();
            p.Name = newName;

            var updateResult = await this.reader.UpdatePlayersAsync(new List<Player>() { p }, "C:\\pesdb");

            Assert.IsNotNull(updateResult);

            // Get players again
            result = await this.reader.ReadPlayersAsync("C:\\pesdb");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            p = result[0];

            Assert.AreEqual(p.Name, newName);
        }

        /// <summary>
        /// Tests the extracting of club data from the PES binary files.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestReadClubsAsync()
        {
            List<Club> result = await this.reader.ReadClubsAsync("C:\\pesdb");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        /// <summary>
        /// Tests the extracting of player team assignment data from the PES binary files.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestReadPlayerAssignments()
        {
            List<TeamAssignment> result = await this.reader.ReadPlayerAssignmentsAsync("C:\\pesdb");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        /// <summary>
        /// Tests the extracting of competition data from the PES binary files.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestReadCompetitionsAsync()
        {
            List<Competition> result = await this.reader.ReadCompetitionsAsync("C:\\pesdb");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        /// <summary>
        /// Tests the extracting of competition entries data from the PES binary files.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestReadCompetitionEntriesAsync()
        {
            List<CompetitionEntry> result = await this.reader.ReadCompetitionEntriesAsync("C:\\pesdb");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
    }
}