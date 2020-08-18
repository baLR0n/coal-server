using COAL.CORE.Models.Competition;
using COAL.CORE.Models.Competition.Matches;
using CoalServer.Services;
using CoalServer.Services.Competitions;
using CoalServer.Services.Matches;
using CoalServer.Services.Tables;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace coal_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompetitionsController : ControllerBase
    {
        private readonly ICompetitionService competitionService;
        private readonly ITableService tableService;
        private readonly IMatchService matchService;

        public CompetitionsController(ICompetitionService competitionService, ITableService tableService, IMatchService matchService)
        {
            this.competitionService = competitionService;
            this.tableService = tableService;
            this.matchService = matchService;
        }

        // GET api/competitions
        [HttpGet]
        public async Task<ActionResult<List<Competition>>> GetAsync()
        {
            return await this.competitionService.GetAsync();
        }

        // GET api/competitions/5
        [HttpGet("{id}", Name = "GetCompetition")]
        public async Task<ActionResult<Competition>> GetAsync(string id)
        {
            Competition competition = await this.competitionService.GetAsync(id);
            if (competition == null)
            {
                return NotFound();
            }

            return competition;
        }

        // GET api/competitions/5/table
        [HttpGet("{id}/table", Name = "GetCompetitionTable")]
        public async Task<ActionResult<Table>> GetTableAsync(string id)
        {
            Table table = await this.tableService.GetFromCompetitonIdAsync(id);
            if (table == null)
            {
                return NotFound();
            }

            return Ok(table);
        }

        // GET api/competitions/5/fixtures
        [HttpGet("{id}/fixtures", Name = "GetCompetitionFixtures")]
        public async Task<ActionResult<List<Match>>> GetFixturesAsync(string id)
        {
            // ToDo: Parameter for Season (-T19 => TXX)
            List<Match> fixtures = await this.matchService.GetFixturesForTableAsync(id + "-T0");

            if (fixtures == null)
            {
                return NotFound();
            }

            return Ok(fixtures);
        }

        // POST api/competitions
        [HttpPost]
        public async Task<ActionResult<Competition>> Create(Competition competition)
        {
            await this.competitionService.CreateAsync(competition);
            return CreatedAtRoute("Competitions", new { id = competition.Id.ToString() }, competition);
        }

        // PUT api/competitions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Competition competitionIn)
        {
            Competition competition = await this.competitionService.GetAsync(id);

            if (competition == null)
            {
                return NotFound();
            }

            await this.competitionService.UpdateAsync(id, competitionIn);
            return NoContent();
        }

        // DELETE api/competitions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            Competition competition = await this.competitionService.GetAsync(id);

            if (competition == null)
            {
                return NotFound();
            }

            await this.competitionService.RemoveAsync(competition.Id);
            return NoContent();
        }
    }
}
