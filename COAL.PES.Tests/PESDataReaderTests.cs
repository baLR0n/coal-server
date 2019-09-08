using COAL.CORE.Models;
using COAL.PES.Data;
using NUnit.Framework;
using System.Collections.Generic;

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
        public async System.Threading.Tasks.Task TestReadPlayersAsync()
        {
            List<Player> result = await this.reader.ReadPlayersAsync("C:\\pesdb");

            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count > 0);
        }
    }
}