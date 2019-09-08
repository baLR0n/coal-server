using COAL.CORE.Models;
using COAL.CORE.Models.Team;
using COAL.PES.Data;
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

        [Test]
        public async Task TestReadPlayersAsync()
        {
            List<Player> result = await this.reader.ReadPlayersAsync("C:\\pesdb");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }

        [Test]
        public async Task TestUpdatePlayerAsync()
        {
            List<Player> result = await this.reader.ReadPlayersAsync("C:\\pesdb");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            Player p = result[0];
            string newName = "Random Name " + DateTime.Now.Millisecond.ToString();
            p.FirstName = newName;

            var updateResult = await this.reader.UpdatePlayersAsync(new List<Player>() { p }, "C:\\pesdb");

            Assert.IsNotNull(updateResult);

            // Get players again
            result = await this.reader.ReadPlayersAsync("C:\\pesdb");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);

            p = result[0];

            Assert.AreEqual(p.FirstName, newName);
        }

        [Test]
        public async Task TestReadClubsAsync()
        {
            List<Club> result = await this.reader.ReadClubsAsync("C:\\pesdb");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
    }
}