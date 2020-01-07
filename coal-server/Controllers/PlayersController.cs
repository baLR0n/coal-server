using COAL.CORE.Models;
using CoalServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace coal_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly PlayerService playerService;

        public PlayersController(PlayerService playerService)
        {
            this.playerService = playerService;
        }

        // GET api/players
        [HttpGet]
        public ActionResult<List<Player>> Get() => this.playerService.Get();

        // GET api/players/5
        [HttpGet("{id}", Name = "GetPlayer")]
        public ActionResult<Player> Get(string id)
        {
            Player player = this.playerService.Get(id);
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
        public ActionResult<Player> Create(Player player)
        {
            this.playerService.Create(player);
            return CreatedAtRoute("GetPlayer", new { id = player.Id.ToString() }, player);
        }

        // PUT api/players/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Player playerIn)
        {
            Player player = this.playerService.Get(id);

            if(player == null)
            {
                return NotFound();
            }

            await this.playerService.UpdateAsync(id, playerIn);
            return NoContent();
        }

        // DELETE api/players/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            Player player = this.playerService.Get(id);

            if (player == null)
            {
                return NotFound();
            }

            this.playerService.Remove(player.Id);
            return NoContent();
        }
    }
}
