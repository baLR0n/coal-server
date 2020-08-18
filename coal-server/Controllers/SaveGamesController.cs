using COAL.CORE.Core.Game;
using COAL.CORE.Models;
using CoalServer.Services;
using CoalServer.Services.Players;
using CoalServer.Services.SaveGames;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace coal_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SaveGamesController : ControllerBase
    {
        private readonly ISaveGameService saveGameService;
        private readonly IPlayerService playerService;

        public SaveGamesController(ISaveGameService saveGameService, IPlayerService playerService)
        {
            this.saveGameService = saveGameService;
            this.playerService = playerService;
        }

        // GET api/saveGames
        [HttpGet]
        public async Task<ActionResult<List<SaveGame>>> GetAsync()
        {
            return await this.saveGameService.GetAsync();
        }

        /// <summary>
        /// Loads a specified save game.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("load/{id}")]
        public async Task<ActionResult<SaveGame>> LoadAsync(string id)
        {
            SaveGame saveGame = await this.saveGameService.GetByNameAsync(id);
            if (saveGame == null)
            {
                return NotFound();
            }

            await this.saveGameService.LoadAsync(saveGame);

            // ToDo: Refresh services
            return Ok(saveGame);
        }

        /// <summary>
        /// Simluates the current day.
        /// </summary>
        /// <returns></returns>
        [HttpGet("simulate")]
        public async Task<ActionResult<DateTime>> SimulateDay()
        {
            SaveGame newSaveGame = await this.saveGameService.IncrementIngameDateAsync();

            return Ok(newSaveGame.InGameDate);
        }

        // GET api/saveGames/5
        /// <summary>
        /// Gets a specified save game
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}", Name = "GetSaveGame")]
        public async Task<ActionResult<SaveGame>> GetAsync(string id)
        {
            SaveGame saveGame = await this.saveGameService.GetByNameAsync(id);
            if(saveGame == null)
            {
                return NotFound();
            }

            return saveGame;
        }

        // POST api/saveGames
        /// <summary>
        /// Creates a new save game.
        /// </summary>
        /// <param name="saveGame"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<SaveGame>> CreateAsync(SaveGame saveGame)
        {
            await this.saveGameService.CreateAsync(saveGame);
            return CreatedAtRoute("GetSaveGame", new { id = saveGame.Id.ToString() }, saveGame);
        }

        // PUT api/saveGames/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, SaveGame saveGameIn)
        {
            SaveGame saveGame = await this.saveGameService.GetAsync(id);

            if(saveGame == null)
            {
                return NotFound();
            }

            await this.saveGameService.UpdateAsync(id, saveGameIn);
            return NoContent();
        }

        // DELETE api/saveGames/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            SaveGame saveGame = await this.saveGameService.GetAsync(id);

            if (saveGame == null)
            {
                return NotFound();
            }

            await this.saveGameService.RemoveAsync(saveGame.Id);
            return NoContent();
        }
    }
}
