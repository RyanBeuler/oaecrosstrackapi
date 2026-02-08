using Microsoft.EntityFrameworkCore;
using OaeCrosstrackApi.Data;
using OaeCrosstrackApi.DTOs.Meets;
using OaeCrosstrackApi.Models;

namespace OaeCrosstrackApi.Services
{
    public interface IMeetService
    {
        Task<IEnumerable<MeetResponseDto>> GetAllAsync(int? sportId = null, int? year = null, bool activeOnly = true);
        Task<MeetResponseDto?> GetByIdAsync(int id);
        Task<MeetResponseDto> CreateAsync(CreateMeetDto dto);
        Task<MeetResponseDto?> UpdateAsync(int id, UpdateMeetDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<int>> GetAvailableYearsAsync();
    }

    public class MeetService : IMeetService
    {
        private readonly ApplicationDbContext _context;

        public MeetService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MeetResponseDto>> GetAllAsync(int? sportId = null, int? year = null, bool activeOnly = true)
        {
            var query = _context.Meets
                .Include(m => m.Sport)
                .AsQueryable();

            if (activeOnly)
            {
                query = query.Where(m => m.IsActive);
            }

            if (sportId.HasValue)
            {
                query = query.Where(m => m.SportId == sportId.Value);
            }

            if (year.HasValue)
            {
                // School year runs Aug to Jul, so year 2025 means Aug 2024 - Jul 2025
                var startDate = new DateTime(year.Value - 1, 8, 1, 0, 0, 0, DateTimeKind.Utc);
                var endDate = new DateTime(year.Value, 7, 31, 23, 59, 59, DateTimeKind.Utc);
                query = query.Where(m => m.MeetDate >= startDate && m.MeetDate <= endDate);
            }

            var meets = await query
                .OrderBy(m => m.MeetDate)
                .ToListAsync();

            return meets.Select(MapToResponseDto);
        }

        public async Task<MeetResponseDto?> GetByIdAsync(int id)
        {
            var meet = await _context.Meets
                .Include(m => m.Sport)
                .FirstOrDefaultAsync(m => m.Id == id);

            return meet == null ? null : MapToResponseDto(meet);
        }

        public async Task<MeetResponseDto> CreateAsync(CreateMeetDto dto)
        {
            var meet = new Meet
            {
                SportId = dto.SportId,
                Name = dto.Name,
                Location = dto.Location,
                MeetDate = dto.MeetDate,
                MeetType = dto.MeetType,
                Opponent = dto.Opponent,
                IsHome = dto.IsHome,
                Notes = dto.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Meets.Add(meet);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            await _context.Entry(meet).Reference(m => m.Sport).LoadAsync();

            return MapToResponseDto(meet);
        }

        public async Task<MeetResponseDto?> UpdateAsync(int id, UpdateMeetDto dto)
        {
            var meet = await _context.Meets
                .Include(m => m.Sport)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (meet == null)
            {
                return null;
            }

            meet.SportId = dto.SportId;
            meet.Name = dto.Name;
            meet.Location = dto.Location;
            meet.MeetDate = dto.MeetDate;
            meet.MeetType = dto.MeetType;
            meet.Opponent = dto.Opponent;
            meet.IsHome = dto.IsHome;
            meet.Notes = dto.Notes;
            meet.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Reload sport if it changed
            await _context.Entry(meet).Reference(m => m.Sport).LoadAsync();

            return MapToResponseDto(meet);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var meet = await _context.Meets.FindAsync(id);
            if (meet == null)
            {
                return false;
            }

            // Soft delete
            meet.IsActive = false;
            meet.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<int>> GetAvailableYearsAsync()
        {
            var years = await _context.Meets
                .Where(m => m.IsActive)
                .Select(m => m.MeetDate.Month >= 8 ? m.MeetDate.Year + 1 : m.MeetDate.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            // Always include current and next school year
            var currentYear = DateTime.Now.Month >= 8 ? DateTime.Now.Year + 1 : DateTime.Now.Year;
            if (!years.Contains(currentYear))
            {
                years.Insert(0, currentYear);
            }
            if (!years.Contains(currentYear + 1))
            {
                years.Insert(0, currentYear + 1);
            }

            return years.OrderByDescending(y => y);
        }

        private static MeetResponseDto MapToResponseDto(Meet meet)
        {
            return new MeetResponseDto
            {
                Id = meet.Id,
                SportId = meet.SportId,
                SportName = meet.Sport?.Name ?? string.Empty,
                Name = meet.Name,
                Location = meet.Location,
                MeetDate = meet.MeetDate,
                MeetType = meet.MeetType,
                Opponent = meet.Opponent,
                IsHome = meet.IsHome,
                Notes = meet.Notes,
                IsActive = meet.IsActive,
                CreatedAt = meet.CreatedAt,
                UpdatedAt = meet.UpdatedAt
            };
        }
    }
}
