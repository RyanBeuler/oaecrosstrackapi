using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OaeCrosstrackApi.DTOs.Records;
using OaeCrosstrackApi.Services;

namespace OaeCrosstrackApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RecordsController : ControllerBase
    {
        private readonly IRecordService _recordService;

        public RecordsController(IRecordService recordService)
        {
            _recordService = recordService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RecordResponseDto>>> GetAll(
            [FromQuery] int? eventId,
            [FromQuery] int? sportId,
            [FromQuery] int? athleteId,
            [FromQuery] string? gender,
            [FromQuery] string? recordType,
            [FromQuery] bool activeOnly = true)
        {
            var query = new RecordQueryDto
            {
                EventId = eventId,
                SportId = sportId,
                AthleteId = athleteId,
                Gender = gender,
                RecordType = recordType,
                ActiveOnly = activeOnly
            };

            var records = await _recordService.GetAllAsync(query);
            return Ok(records);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<RecordResponseDto>> GetById(int id)
        {
            var record = await _recordService.GetByIdAsync(id);

            if (record == null)
            {
                return NotFound();
            }

            return Ok(record);
        }

        [HttpGet("event/{eventId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RecordResponseDto>>> GetByEvent(int eventId)
        {
            var records = await _recordService.GetByEventAsync(eventId);
            return Ok(records);
        }

        [HttpGet("sport/{sportId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RecordResponseDto>>> GetBySport(int sportId)
        {
            var records = await _recordService.GetBySportAsync(sportId);
            return Ok(records);
        }

        [HttpGet("leaderboard/{eventId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<RecordResponseDto>>> GetLeaderboard(
            int eventId,
            [FromQuery] string gender,
            [FromQuery] int top = 10)
        {
            if (string.IsNullOrEmpty(gender))
            {
                return BadRequest("Gender is required for leaderboard");
            }

            var records = await _recordService.GetLeaderboardAsync(eventId, gender, top);
            return Ok(records);
        }

        [HttpPost]
        public async Task<ActionResult<RecordResponseDto>> Create(CreateRecordDto dto)
        {
            var record = await _recordService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = record.Id }, record);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<RecordResponseDto>> Update(int id, UpdateRecordDto dto)
        {
            var record = await _recordService.UpdateAsync(id, dto);

            if (record == null)
            {
                return NotFound();
            }

            return Ok(record);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _recordService.DeleteAsync(id);

            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
