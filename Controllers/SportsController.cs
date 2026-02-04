using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OaeCrosstrackApi.DTOs.Sports;
using OaeCrosstrackApi.Services;

namespace OaeCrosstrackApi.Controllers
{
    [Route("api/sports")]
    [ApiController]
    [Authorize]
    public class SportsController : ControllerBase
    {
        private readonly ISportService _sportService;

        public SportsController(ISportService sportService)
        {
            _sportService = sportService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var sports = await _sportService.GetAllAsync(includeInactive);
            return Ok(sports);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var sport = await _sportService.GetByIdAsync(id);

            if (sport == null)
            {
                return NotFound(new { message = "Sport not found" });
            }

            return Ok(sport);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateSportDto dto)
        {
            var sport = await _sportService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = sport.Id }, sport);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateSportDto dto)
        {
            var sport = await _sportService.UpdateAsync(id, dto);

            if (sport == null)
            {
                return NotFound(new { message = "Sport not found" });
            }

            return Ok(sport);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _sportService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Sport not found" });
            }

            return NoContent();
        }
    }
}
