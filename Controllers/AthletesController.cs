using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OaeCrosstrackApi.DTOs.Athletes;
using OaeCrosstrackApi.Services;

namespace OaeCrosstrackApi.Controllers
{
    [Route("api/athletes")]
    [ApiController]
    [Authorize]
    public class AthletesController : ControllerBase
    {
        private readonly IAthleteService _athleteService;

        public AthletesController(IAthleteService athleteService)
        {
            _athleteService = athleteService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var athletes = await _athleteService.GetAllAsync(includeInactive);
            return Ok(athletes);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var athlete = await _athleteService.GetByIdAsync(id);

            if (athlete == null)
            {
                return NotFound(new { message = "Athlete not found" });
            }

            return Ok(athlete);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateAthleteDto dto)
        {
            var athlete = await _athleteService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = athlete.Id }, athlete);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAthleteDto dto)
        {
            var athlete = await _athleteService.UpdateAsync(id, dto);

            if (athlete == null)
            {
                return NotFound(new { message = "Athlete not found" });
            }

            return Ok(athlete);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _athleteService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Athlete not found" });
            }

            return NoContent();
        }
    }
}
