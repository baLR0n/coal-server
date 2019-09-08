using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace COAL.CORE.Models
{
    public class CoalDatabaseSettings : ICoalDatabaseSettings
    {
        public string SaveGamesCollectionName { get; set; }
        public string PlayersCollectionName { get; set; }
        public string ClubsCollectionName { get; set; }
        public string TeamsCollectionName { get; set; }
        public string ContractsCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string MainDatabase { get; set; }
        public string DatabaseName { get; set; }
        public string CompetitionsCollectionName { get; set; }
        public string TablesCollectionName { get; set; }
        public string MatchesCollectionName { get; set; }
    }

    public interface ICoalDatabaseSettings
    {
        string SaveGamesCollectionName { get; set; }
        string PlayersCollectionName { get; set; }
        string ClubsCollectionName { get; set; }
        string TeamsCollectionName { get; set; }
        string ContractsCollectionName { get; set; }
        string ConnectionString { get; set; }
        string MainDatabase { get; set; }
        string DatabaseName { get; set; }
        string CompetitionsCollectionName { get; set; }
        string TablesCollectionName { get; set; }
        string MatchesCollectionName { get; set; }
    }
}
