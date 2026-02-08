using Microsoft.EntityFrameworkCore;
using OaeCrosstrackApi.Data;
using OaeCrosstrackApi.DTOs.Dash;
using OaeCrosstrackApi.Models;

namespace OaeCrosstrackApi.Services
{
    public interface IDashService
    {
        Task<IEnumerable<DashResponseDto>> GetAllAsync();
        Task<DashResponseDto?> GetByYearAsync(int year);
        Task<DashResponseDto> UpsertAsync(int year, UpsertDashDto dto);
        Task<IEnumerable<int>> GetAvailableYearsAsync();
        Task<DashFileResponseDto> UploadFileAsync(int year, IFormFile file, string category, string? description);
        Task<(Stream Stream, string ContentType, string FileName)?> DownloadFileAsync(int fileId);
        Task<bool> DeleteFileAsync(int fileId);
    }

    public class DashService : IDashService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public DashService(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<IEnumerable<DashResponseDto>> GetAllAsync()
        {
            var items = await _context.DashContents
                .Include(d => d.Files!.Where(f => f.IsActive))
                .Where(d => d.IsActive)
                .OrderByDescending(d => d.Year)
                .ToListAsync();

            return items.Select(MapToResponseDto);
        }

        public async Task<DashResponseDto?> GetByYearAsync(int year)
        {
            var item = await _context.DashContents
                .Include(d => d.Files!.Where(f => f.IsActive))
                .FirstOrDefaultAsync(d => d.Year == year && d.IsActive);

            return item == null ? null : MapToResponseDto(item);
        }

        public async Task<DashResponseDto> UpsertAsync(int year, UpsertDashDto dto)
        {
            var existing = await _context.DashContents
                .Include(d => d.Files!.Where(f => f.IsActive))
                .FirstOrDefaultAsync(d => d.Year == year);

            if (existing != null)
            {
                existing.RegistrationMarkdown = dto.RegistrationMarkdown;
                existing.PastResultsMarkdown = dto.PastResultsMarkdown;
                existing.CourseMapMarkdown = dto.CourseMapMarkdown;
                existing.IsActive = true;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                existing = new DashContent
                {
                    Year = year,
                    RegistrationMarkdown = dto.RegistrationMarkdown,
                    PastResultsMarkdown = dto.PastResultsMarkdown,
                    CourseMapMarkdown = dto.CourseMapMarkdown,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.DashContents.Add(existing);
            }

            await _context.SaveChangesAsync();

            // Reload with navigation properties
            var saved = await _context.DashContents
                .Include(d => d.Files!.Where(f => f.IsActive))
                .FirstAsync(d => d.Id == existing.Id);

            return MapToResponseDto(saved);
        }

        public async Task<IEnumerable<int>> GetAvailableYearsAsync()
        {
            return await _context.DashContents
                .Where(d => d.IsActive)
                .OrderByDescending(d => d.Year)
                .Select(d => d.Year)
                .ToListAsync();
        }

        public async Task<DashFileResponseDto> UploadFileAsync(int year, IFormFile file, string category, string? description)
        {
            // Get or create DashContent for this year
            var dashContent = await _context.DashContents
                .FirstOrDefaultAsync(d => d.Year == year);

            if (dashContent == null)
            {
                dashContent = new DashContent
                {
                    Year = year,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.DashContents.Add(dashContent);
                await _context.SaveChangesAsync();
            }

            // Create upload directory
            var uploadDir = Path.Combine(_environment.ContentRootPath, "uploads", "dash", year.ToString());
            Directory.CreateDirectory(uploadDir);

            // Generate unique filename
            var storedFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadDir, storedFileName);

            // Save file to disk
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Save metadata to database
            var dashFile = new DashFile
            {
                DashContentId = dashContent.Id,
                OriginalFileName = file.FileName,
                StoredFileName = storedFileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                Category = category,
                Description = description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.DashFiles.Add(dashFile);
            await _context.SaveChangesAsync();

            return MapToFileResponseDto(dashFile);
        }

        public async Task<(Stream Stream, string ContentType, string FileName)?> DownloadFileAsync(int fileId)
        {
            var dashFile = await _context.DashFiles
                .Include(f => f.DashContent)
                .FirstOrDefaultAsync(f => f.Id == fileId && f.IsActive);

            if (dashFile == null || dashFile.DashContent == null)
            {
                return null;
            }

            var filePath = Path.Combine(
                _environment.ContentRootPath,
                "uploads",
                "dash",
                dashFile.DashContent.Year.ToString(),
                dashFile.StoredFileName);

            if (!File.Exists(filePath))
            {
                return null;
            }

            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            return (stream, dashFile.ContentType, dashFile.OriginalFileName);
        }

        public async Task<bool> DeleteFileAsync(int fileId)
        {
            var dashFile = await _context.DashFiles.FindAsync(fileId);

            if (dashFile == null)
            {
                return false;
            }

            // Soft delete
            dashFile.IsActive = false;
            dashFile.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        private static DashResponseDto MapToResponseDto(DashContent item)
        {
            return new DashResponseDto
            {
                Id = item.Id,
                Year = item.Year,
                RegistrationMarkdown = item.RegistrationMarkdown,
                PastResultsMarkdown = item.PastResultsMarkdown,
                CourseMapMarkdown = item.CourseMapMarkdown,
                IsActive = item.IsActive,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt,
                Files = item.Files?.Where(f => f.IsActive)
                    .Select(MapToFileResponseDto)
                    .ToList() ?? new List<DashFileResponseDto>()
            };
        }

        private static DashFileResponseDto MapToFileResponseDto(DashFile file)
        {
            return new DashFileResponseDto
            {
                Id = file.Id,
                DashContentId = file.DashContentId,
                OriginalFileName = file.OriginalFileName,
                ContentType = file.ContentType,
                FileSize = file.FileSize,
                Category = file.Category,
                Description = file.Description,
                IsActive = file.IsActive,
                CreatedAt = file.CreatedAt,
                UpdatedAt = file.UpdatedAt
            };
        }
    }
}
