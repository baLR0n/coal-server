using System.Threading.Tasks;
using COAL.CORE.Core.Game;

namespace CoalServer.Services.SaveGames
{
    public interface ISaveGameService : ICrudService<SaveGame>
    {
        Task<SaveGame> GetByNameAsync(string saveGameId);
        Task<SaveGame> IncrementIngameDateAsync();
        Task LoadAsync(SaveGame saveGame, bool init = false);
    }
}