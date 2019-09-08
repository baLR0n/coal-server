using COAL.CORE.Models.Team;
using CoalServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace coal_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly TeamService teamService;

        public TeamsController(TeamService teamService)
        {
            this.teamService = teamService;
        }

        // GET api/teams
        [HttpGet]
        public ActionResult<List<Team>> Get() => this.teamService.Get();

        // GET api/teams/5
        [HttpGet("{id}", Name = "GetTeam")]
        public ActionResult<Team> Get(string id)
        {
            Team team = this.teamService.Get(id);
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
            this.teamService.Create(team);
            return CreatedAtRoute("Teams", new { id = team.Id.ToString() }, team);
        }

        // PUT api/teams/5
        [HttpPut("{id}")]
        public IActionResult Update(string id, Team teamIn)
        {
            Team team = this.teamService.Get(id);

            if (team == null)
            {
                return NotFound();
            }

            this.teamService.Update(id, teamIn);
            return NoContent();
        }

        // DELETE api/teams/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            Team team = this.teamService.Get(id);

            if (team == null)
            {
                return NotFound();
            }

            this.teamService.Remove(team.Id);
            return NoContent();
        }
    }
}
