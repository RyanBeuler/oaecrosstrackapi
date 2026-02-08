using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OaeCrosstrackApi.DTOs.Dash;
using OaeCrosstrackApi.Services;

namespace OaeCrosstrackApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashController : ControllerBase
    {
        private readonly IDashService _dashService;

        public DashController(IDashService dashService)
        {
            _dashService = dashService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<DashResponseDto>>> GetAll()
        {
            var items = await _dashService.GetAllAsync();
            return Ok(items);
        }

        [HttpGet("years")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<int>>> GetAvailableYears()
        {
            var years = await _dashService.GetAvailableYearsAsync();
            return Ok(years);
        }

        [HttpGet("year/{year}")]
        [AllowAnonymous]
        public async Task<ActionResult<DashResponseDto>> GetByYear(int year)
        {
            var item = await _dashService.GetByYearAsync(year);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPut("year/{year}")]
        public async Task<ActionResult<DashResponseDto>> Upsert(int year, UpsertDashDto dto)
        {
            var item = await _dashService.UpsertAsync(year, dto);
            return Ok(item);
        }

        [HttpPost("year/{year}/files")]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10MB max
        public async Task<ActionResult<DashFileResponseDto>> UploadFile(
            int year,
            IFormFile file,
            [FromForm] string category,
            [FromForm] string? description)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file provided");
            }

            // Validate category
            var validCategories = new[] { "Registration", "PastResults", "CourseMap" };
            if (string.IsNullOrEmpty(category) || !validCategories.Contains(category))
            {
                return BadRequest("Invalid category. Must be: Registration, PastResults, or CourseMap.");
            }

            // Validate file type
            var allowedTypes = new[]
            {
                "application/pdf",
                "application/msword",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                "image/jpeg",
                "image/png",
                "image/gif",
                "image/webp"
            };

            if (!allowedTypes.Contains(file.ContentType.ToLower()))
            {
                return BadRequest("File type not allowed. Accepted: PDF, Word documents, and images (JPEG, PNG, GIF, WebP).");
            }

            var result = await _dashService.UploadFileAsync(year, file, category, description);
            return Ok(result);
        }

        [HttpGet("files/{fileId}/download")]
        [AllowAnonymous]
        public async Task<IActionResult> DownloadFile(int fileId)
        {
            var result = await _dashService.DownloadFileAsync(fileId);

            if (result == null)
            {
                return NotFound();
            }

            var (stream, contentType, fileName) = result.Value;
            return File(stream, contentType, fileName);
        }

        [HttpDelete("files/{fileId}")]
        public async Task<IActionResult> DeleteFile(int fileId)
        {
            var success = await _dashService.DeleteFileAsync(fileId);

            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
