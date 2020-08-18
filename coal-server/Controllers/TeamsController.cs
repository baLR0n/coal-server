using COAL.CORE.Models.Team;
using CoalServer.Services;
using CoalServer.Services.Teams;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace coal_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly ITeamService teamService;

        public TeamsController(ITeamService teamService)
        {
            this.teamService = teamService;
        }

        // GET api/teams
        [HttpGet]
        public async Task<ActionResult<List<Team>>> GetAsync()
        {
            return await this.teamService.GetAsync();
        }

        // GET api/teams/5
        [HttpGet("{id}", Name = "GetTeam")]
        public async Task<ActionResult<Team>> GetAsync(string id)
        {
            Team team = await this.teamService.GetAsync(id);
            if (team == null)
            {
                return NotFound();
            }

            return team;
        }

        // POST api/teams
        [HttpPost]
        public ActionResult<Team> Create(Team team)
        {
            this.teamService.CreateAsync(team);
            return CreatedAtRoute("Teams", new { id = team.Id.ToString() }, team);
        }

        // PUT api/teams/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(string id, Team teamIn)
        {
            Team team = await this.teamService.GetAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            this.teamService.UpdateAsync(id, teamIn);
            return NoContent();
        }

        // DELETE api/teams/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            Team team = await this.teamService.GetAsync(id);

            if (team == null)
            {
                return NotFound();
            }

            this.teamService.RemoveAsync(team.Id);
            return NoContent();
        }
    }
}
