using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OaeCrosstrackApi.DTOs.History;
using OaeCrosstrackApi.Services;

namespace OaeCrosstrackApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<HistoryResponseDto>>> GetAll()
        {
            var items = await _historyService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("sport/{sportId}")]
        [AllowAnonymous]
        public async Task<ActionResult<HistoryResponseDto>> GetBySportId(int sportId)
        {
            var item = await _historyService.GetBySportIdAsync(sportId);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPut("sport/{sportId}")]
        public async Task<ActionResult<HistoryResponseDto>> Upsert(int sportId, UpsertHistoryDto dto)
        {
            var item = await _historyService.UpsertAsync(sportId, dto);
            return Ok(item);
        }
    }
}
