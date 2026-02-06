using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OaeCrosstrackApi.DTOs.Meets;
using OaeCrosstrackApi.Services;

namespace OaeCrosstrackApi.Controllers
{
    [Route("api/meets")]
    [ApiController]
    [Authorize]
    public class MeetsController : ControllerBase
    {
        private readonly IMeetService _meetService;

        public MeetsController(IMeetService meetService)
        {
            _meetService = meetService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? sportId,
            [FromQuery] int? year,
            [FromQuery] bool? activeOnly = true)
        {
            var meets = await _meetService.GetAllAsync(sportId, year, activeOnly ?? true);
            return Ok(meets);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var meet = await _meetService.GetByIdAsync(id);

            if (meet == null)
            {
                return NotFound(new { message = "Meet not found" });
            }

            return Ok(meet);
        }

        [HttpGet("years")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAvailableYears()
        {
            var years = await _meetService.GetAvailableYearsAsync();
            return Ok(years);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateMeetDto dto)
        {
            var meet = await _meetService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = meet.Id }, meet);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMeetDto dto)
        {
            var meet = await _meetService.UpdateAsync(id, dto);

            if (meet == null)
            {
                return NotFound(new { message = "Meet not found" });
            }

            return Ok(meet);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _meetService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Meet not found" });
            }

            return NoContent();
        }
    }
}
