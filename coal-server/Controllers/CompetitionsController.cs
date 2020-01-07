using COAL.CORE.Models.Competition;
using COAL.CORE.Models.Competition.Matches;
using CoalServer.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace coal_server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompetitionsController : ControllerBase
    {
        private readonly CompetitionService competitionService;
        private readonly TableService tableService;
        private readonly MatchService matchService;

        public CompetitionsController(CompetitionService competitionService, TableService tableService, MatchService matchService)
        {
            this.competitionService = competitionService;
            this.tableService = tableService;
            this.matchService = matchService;
        }

        // GET api/competitions
        [HttpGet]
        public ActionResult<List<Competition>> Get() => this.competitionService.Get();

        // GET api/competitions/5
        [HttpGet("{id}", Name = "GetCompetition")]
        public ActionResult<Competition> Get(string id)
        {
            Competition competition = this.competitionService.Get(id);
            if (competition == null)
            {
                return NotFound();
            }

            return competition;
        }

        // GET api/competitions/5/table
        [HttpGet("{id}/table", Name = "GetCompetitionTable")]
        public ActionResult<Table> GetTable(string id)
        {
            Table table = this.tableService.GetFromCompetitonId(id);
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
        public ActionResult<Competition> Create(Competition competition)
        {
            this.competitionService.Create(competition);
            return CreatedAtRoute("Competitions", new { id = competition.Id.ToString() }, competition);
        }

        // PUT api/competitions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Competition competitionIn)
        {
            Competition competition = this.competitionService.Get(id);

            if (competition == null)
            {
                return NotFound();
            }

            await this.competitionService.UpdateAsync(id, competitionIn);
            return NoContent();
        }

        // DELETE api/competitions/5
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            Competition competition = this.competitionService.Get(id);

            if (competition == null)
            {
                return NotFound();
            }

            this.competitionService.Remove(competition.Id);
            return NoContent();
        }
    }
}
