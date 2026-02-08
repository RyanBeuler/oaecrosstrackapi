using Microsoft.EntityFrameworkCore;
using OaeCrosstrackApi.Data;
using OaeCrosstrackApi.DTOs.History;
using OaeCrosstrackApi.Models;

namespace OaeCrosstrackApi.Services
{
    public interface IHistoryService
    {
        Task<IEnumerable<HistoryResponseDto>> GetAllAsync();
        Task<HistoryResponseDto?> GetBySportIdAsync(int sportId);
        Task<HistoryResponseDto> UpsertAsync(int sportId, UpsertHistoryDto dto);
    }

    public class HistoryService : IHistoryService
    {
        private readonly ApplicationDbContext _context;

        public HistoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<HistoryResponseDto>> GetAllAsync()
        {
            var items = await _context.HistoryContents
                .Include(h => h.Sport)
                .Where(h => h.IsActive)
                .OrderBy(h => h.Sport!.DisplayOrder)
                .ToListAsync();

            return items.Select(MapToResponseDto);
        }

        public async Task<HistoryResponseDto?> GetBySportIdAsync(int sportId)
        {
            var item = await _context.HistoryContents
                .Include(h => h.Sport)
                .FirstOrDefaultAsync(h => h.SportId == sportId && h.IsActive);

            return item == null ? null : MapToResponseDto(item);
        }

        public async Task<HistoryResponseDto> UpsertAsync(int sportId, UpsertHistoryDto dto)
        {
            var existing = await _context.HistoryContents
                .Include(h => h.Sport)
                .FirstOrDefaultAsync(h => h.SportId == sportId);

            if (existing != null)
            {
                existing.MarkdownContent = dto.MarkdownContent;
                existing.IsActive = true;
                existing.UpdatedAt = DateTime.UtcNow;
            }
            else
            {
                existing = new HistoryContent
                {
                    SportId = sportId,
                    MarkdownContent = dto.MarkdownContent,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.HistoryContents.Add(existing);
            }

            await _context.SaveChangesAsync();

            // Reload with navigation properties
            var saved = await _context.HistoryContents
                .Include(h => h.Sport)
                .FirstAsync(h => h.Id == existing.Id);

            return MapToResponseDto(saved);
        }

        private static HistoryResponseDto MapToResponseDto(HistoryContent item)
        {
            return new HistoryResponseDto
            {
                Id = item.Id,
                SportId = item.SportId,
                SportName = item.Sport?.Name ?? string.Empty,
                MarkdownContent = item.MarkdownContent,
                IsActive = item.IsActive,
                CreatedAt = item.CreatedAt,
                UpdatedAt = item.UpdatedAt
            };
        }
    }
}
