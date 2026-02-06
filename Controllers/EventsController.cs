using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OaeCrosstrackApi.DTOs.Events;
using OaeCrosstrackApi.Services;

namespace OaeCrosstrackApi.Controllers
{
    [Route("api/events")]
    [ApiController]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;

        public EventsController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll([FromQuery] bool includeInactive = false)
        {
            var events = await _eventService.GetAllAsync(includeInactive);
            return Ok(events);
        }

        [HttpGet("sport/{sportId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetBySportId(int sportId, [FromQuery] bool includeInactive = false)
        {
            var events = await _eventService.GetBySportIdAsync(sportId, includeInactive);
            return Ok(events);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var evt = await _eventService.GetByIdAsync(id);

            if (evt == null)
            {
                return NotFound(new { message = "Event not found" });
            }

            return Ok(evt);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
        {
            var evt = await _eventService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = evt.Id }, evt);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto dto)
        {
            var evt = await _eventService.UpdateAsync(id, dto);

            if (evt == null)
            {
                return NotFound(new { message = "Event not found" });
            }

            return Ok(evt);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _eventService.DeleteAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Event not found" });
            }

            return NoContent();
        }
    }
}
