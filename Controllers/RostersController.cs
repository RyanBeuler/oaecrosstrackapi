using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OaeCrosstrackApi.DTOs.Rosters;
using OaeCrosstrackApi.Services;

namespace OaeCrosstrackApi.Controllers
{
    [Route("api/rosters")]
    [ApiController]
    [Authorize]
    public class RostersController : ControllerBase
    {
        private readonly IRosterService _rosterService;

        public RostersController(IRosterService rosterService)
        {
            _rosterService = rosterService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] int? sportId, [FromQuery] int? year)
        {
            var entries = await _rosterService.GetAllAsync(sportId, year);
            return Ok(entries);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var entry = await _rosterService.GetByIdAsync(id);

            if (entry == null)
            {
                return NotFound(new { message = "Roster entry not found" });
            }

            return Ok(entry);
        }

        [HttpGet("years")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableYears()
        {
            var years = await _rosterService.GetAvailableYearsAsync();
            return Ok(years);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateRosterEntryDto dto)
        {
            try
            {
                var entry = await _rosterService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = entry.Id }, entry);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkAdd([FromBody] BulkAddRosterEntriesDto dto)
        {
            var entries = await _rosterService.BulkAddAsync(dto);
            return Ok(entries);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _rosterService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Roster entry not found" });
            }

            return NoContent();
        }

        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDelete([FromBody] BulkDeleteDto dto)
        {
            var count = await _rosterService.BulkDeleteAsync(dto.SportId, dto.Year, dto.AthleteIds);
            return Ok(new { deletedCount = count });
        }
    }

    public class BulkDeleteDto
    {
        public int SportId { get; set; }
        public int Year { get; set; }
        public required int[] AthleteIds { get; set; }
    }
}
