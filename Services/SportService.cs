using Microsoft.EntityFrameworkCore;
using OaeCrosstrackApi.Data;
using OaeCrosstrackApi.DTOs.Sports;
using OaeCrosstrackApi.Models;

namespace OaeCrosstrackApi.Services
{
    public interface ISportService
    {
        Task<IEnumerable<SportResponseDto>> GetAllAsync(bool includeInactive = false);
        Task<SportResponseDto?> GetByIdAsync(int id);
        Task<SportResponseDto> CreateAsync(CreateSportDto dto);
        Task<SportResponseDto?> UpdateAsync(int id, UpdateSportDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public class SportService : ISportService
    {
        private readonly ApplicationDbContext _context;

        public SportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SportResponseDto>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.Sports.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(s => s.IsActive);
            }

            var sports = await query
                .OrderBy(s => s.DisplayOrder)
                .ThenBy(s => s.Name)
                .ToListAsync();

            return sports.Select(MapToResponseDto);
        }

        public async Task<SportResponseDto?> GetByIdAsync(int id)
        {
            var sport = await _context.Sports.FindAsync(id);
            return sport == null ? null : MapToResponseDto(sport);
        }

        public async Task<SportResponseDto> CreateAsync(CreateSportDto dto)
        {
            var sport = new Sport
            {
                Name = dto.Name,
                Season = dto.Season,
                DisplayOrder = dto.DisplayOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Sports.Add(sport);
            await _context.SaveChangesAsync();

            return MapToResponseDto(sport);
        }

        public async Task<SportResponseDto?> UpdateAsync(int id, UpdateSportDto dto)
        {
            var sport = await _context.Sports.FindAsync(id);
            if (sport == null)
            {
                return null;
            }

            sport.Name = dto.Name;
            sport.Season = dto.Season;
            sport.DisplayOrder = dto.DisplayOrder;
            sport.IsActive = dto.IsActive;
            sport.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponseDto(sport);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sport = await _context.Sports.FindAsync(id);
            if (sport == null)
            {
                return false;
            }

            // Soft delete
            sport.IsActive = false;
            sport.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        private static SportResponseDto MapToResponseDto(Sport sport)
        {
            return new SportResponseDto
            {
                Id = sport.Id,
                Name = sport.Name ?? string.Empty,
                Season = sport.Season ?? string.Empty,
                DisplayOrder = sport.DisplayOrder,
                IsActive = sport.IsActive,
                CreatedAt = sport.CreatedAt,
                UpdatedAt = sport.UpdatedAt
            };
        }
    }
}
