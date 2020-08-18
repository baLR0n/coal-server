using COAL.CORE.Models;
using CoalServer.Services.Players;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace coal_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IPlayerService playerService;

        public PlayersController(IPlayerService playerService)
        {
            this.playerService = playerService;
        }

        // GET api/players
        [HttpGet]
        public async Task<ActionResult<List<Player>>> Get() => await this.playerService.GetAsync();

        // GET api/players/5
        [HttpGet("{id}", Name = "GetPlayer")]
        public async Task<ActionResult<Player>> Get(string id)
        {
            Player player = await this.playerService.GetAsync(id);
            if(player == null)
            {
                return NotFound();
            }

            return player;
        }

        // GET api/players/team/5
        [HttpGet("team/{teamId}", Name = "GetPlayerFromTeam")]
        public async Task<ActionResult<List<Player>>> GetPlayerFromTeam(string teamId)
        {
            List<Player> players = await this.playerService.GetFromTeamAsync(teamId);
            if (players == null)
            {
                return NotFound();
            }

            return players;
        }

        // POST api/players
        [HttpPost]
        public async Task<ActionResult<Player>> Create(Player player)
        {
            await this.playerService.CreateAsync(player);
            return CreatedAtRoute("GetPlayer", new { id = player.Id.ToString() }, player);
        }

        // PUT api/players/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Player playerIn)
        {
            Player player = await this.playerService.GetAsync(id);

            if(player == null)
            {
                return NotFound();
            }

            await this.playerService.UpdateAsync(id, playerIn);
            return NoContent();
        }

        // DELETE api/players/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            Player player = await this.playerService.GetAsync(id);

            if (player == null)
            {
                return NotFound();
            }

            await this.playerService.RemoveAsync(player.Id);
            return NoContent();
        }
    }
}
