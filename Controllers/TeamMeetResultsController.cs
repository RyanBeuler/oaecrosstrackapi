using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OaeCrosstrackApi.DTOs.TeamMeetResults;
using OaeCrosstrackApi.Services;

namespace OaeCrosstrackApi.Controllers
{
    [Route("api/team-results")]
    [ApiController]
    [Authorize]
    public class TeamMeetResultsController : ControllerBase
    {
        private readonly ITeamMeetResultService _teamMeetResultService;

        public TeamMeetResultsController(ITeamMeetResultService teamMeetResultService)
        {
            _teamMeetResultService = teamMeetResultService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? sportId,
            [FromQuery] int? year,
            [FromQuery] string? gender,
            [FromQuery] bool? activeOnly = true)
        {
            var results = await _teamMeetResultService.GetAllAsync(sportId, year, gender, activeOnly ?? true);
            return Ok(results);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _teamMeetResultService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = "Team meet result not found" });
            }

            return Ok(result);
        }

        [HttpGet("standings")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStandings(
            [FromQuery] int sportId,
            [FromQuery] int year,
            [FromQuery] string gender)
        {
            var standings = await _teamMeetResultService.GetStandingsAsync(sportId, year, gender);
            return Ok(standings);
        }

        [HttpGet("teams")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDistinctTeams(
            [FromQuery] int sportId,
            [FromQuery] int year)
        {
            var teams = await _teamMeetResultService.GetDistinctTeamsAsync(sportId, year);
            return Ok(teams);
        }

        [HttpGet("years")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableYears()
        {
            var years = await _teamMeetResultService.GetAvailableYearsAsync();
            return Ok(years);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTeamMeetResultDto dto)
        {
            var result = await _teamMeetResultService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTeamMeetResultDto dto)
        {
            var result = await _teamMeetResultService.UpdateAsync(id, dto);

            if (result == null)
            {
                return NotFound(new { message = "Team meet result not found" });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _teamMeetResultService.DeleteAsync(id);

            if (!success)
            {
                return NotFound(new { message = "Team meet result not found" });
            }

            return NoContent();
        }
    }
}
