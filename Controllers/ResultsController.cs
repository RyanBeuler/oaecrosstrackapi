using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OaeCrosstrackApi.DTOs.Results;
using OaeCrosstrackApi.Services;

namespace OaeCrosstrackApi.Controllers
{
    [Route("api/results")]
    [ApiController]
    [Authorize]
    public class ResultsController : ControllerBase
    {
        private readonly IResultService _resultService;

        public ResultsController(IResultService resultService)
        {
            _resultService = resultService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? athleteId,
            [FromQuery] int? meetId,
            [FromQuery] int? eventId,
            [FromQuery] int? sportId,
            [FromQuery] bool? activeOnly = true)
        {
            var results = await _resultService.GetAllAsync(athleteId, meetId, eventId, sportId, activeOnly ?? true);
            return Ok(results);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _resultService.GetByIdAsync(id);

            if (result == null)
            {
                return NotFound(new { message = "Result not found" });
            }

            return Ok(result);
        }

        [HttpGet("athlete/{athleteId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByAthlete(int athleteId)
        {
            var results = await _resultService.GetByAthleteAsync(athleteId);
            return Ok(results);
        }

        [HttpGet("meet/{meetId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByMeet(int meetId)
        {
            var results = await _resultService.GetByMeetAsync(meetId);
            return Ok(results);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateResultDto dto)
        {
            var result = await _resultService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPost("bulk")]
        public async Task<IActionResult> BulkCreate([FromBody] BulkCreateResultDto dto)
        {
            var results = await _resultService.BulkCreateAsync(dto);
            return Ok(results);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateResultDto dto)
        {
            var result = await _resultService.UpdateAsync(id, dto);

            if (result == null)
            {
                return NotFound(new { message = "Result not found" });
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _resultService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new { message = "Result not found" });
            }

            return NoContent();
        }
    }
}
