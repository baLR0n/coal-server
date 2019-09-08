using COAL.CORE.Core.Game;
using COAL.CORE.Models;
using CoalServer.Services;
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
        private readonly SaveGameService saveGameService;
        private readonly PlayerService playerService;

        public SaveGamesController(SaveGameService saveGameService, PlayerService playerService)
        {
            this.saveGameService = saveGameService;
            this.playerService = playerService;
        }

        // GET api/saveGames
        [HttpGet]
        public ActionResult<List<SaveGame>> Get() => this.saveGameService.Get();

        [HttpGet("load/{id}")]
        public async Task<ActionResult<SaveGame>> LoadAsync(string id)
        {
            SaveGame saveGame = this.saveGameService.GetByName(id);
            if (saveGame == null)
            {
                return NotFound();
            }

            await this.saveGameService.LoadAsync(id);

            // ToDo: Refresh services
            return Ok(saveGame);
        }

        [HttpGet("simulate")]
        public async Task<ActionResult<DateTime>> SimulateDay()
        {
            SaveGame newSaveGame = await this.saveGameService.IncrementIngameDate();

            return Ok(newSaveGame.InGameDate);
        }

        // GET api/saveGames/5
        [HttpGet("{id}", Name = "GetSaveGame")]
        public ActionResult<SaveGame> Get(string id)
        {
            SaveGame saveGame = this.saveGameService.GetByName(id);
            if(saveGame == null)
            {
                return NotFound();
            }

            return saveGame;
        }

        // POST api/saveGames
        [HttpPost]
        public async Task<ActionResult<SaveGame>> CreateAsync(SaveGame saveGame)
        {
            await this.saveGameService.CreateAsync(saveGame);
            return CreatedAtRoute("GetSaveGame", new { id = saveGame.Id.ToString() }, saveGame);
        }

        // PUT api/saveGames/5
        [HttpPut("{id}")]
        public IActionResult Update(string id, SaveGame saveGameIn)
        {
            SaveGame saveGame = this.saveGameService.Get(id);

            if(saveGame == null)
            {
                return NotFound();
            }

            this.saveGameService.Update(id, saveGameIn);
            return NoContent();
        }

        // DELETE api/saveGames/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            SaveGame saveGame = this.saveGameService.Get(id);

            if (saveGame == null)
            {
                return NotFound();
            }

            this.saveGameService.Remove(saveGame.Id);
            return NoContent();
        }
    }
}
