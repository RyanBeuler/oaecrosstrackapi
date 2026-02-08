using Microsoft.EntityFrameworkCore;
using OaeCrosstrackApi.Data;
using OaeCrosstrackApi.DTOs.Events;
using OaeCrosstrackApi.Models;

namespace OaeCrosstrackApi.Services
{
    public interface IEventService
    {
        Task<IEnumerable<EventResponseDto>> GetAllAsync(bool includeInactive = false);
        Task<IEnumerable<EventResponseDto>> GetBySportIdAsync(int sportId, bool includeInactive = false);
        Task<EventResponseDto?> GetByIdAsync(int id);
        Task<EventResponseDto> CreateAsync(CreateEventDto dto);
        Task<EventResponseDto?> UpdateAsync(int id, UpdateEventDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public class EventService : IEventService
    {
        private readonly ApplicationDbContext _context;

        public EventService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EventResponseDto>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.Events
                .Include(e => e.Sport)
                .AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(e => e.IsActive);
            }

            var events = await query
                .OrderBy(e => e.SportId)
                .ThenBy(e => e.SortOrder)
                .ThenBy(e => e.Name)
                .ToListAsync();

            return events.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<EventResponseDto>> GetBySportIdAsync(int sportId, bool includeInactive = false)
        {
            var query = _context.Events
                .Include(e => e.Sport)
                .Where(e => e.SportId == sportId);

            if (!includeInactive)
            {
                query = query.Where(e => e.IsActive);
            }

            var events = await query
                .OrderBy(e => e.SortOrder)
                .ThenBy(e => e.Name)
                .ToListAsync();

            return events.Select(MapToResponseDto);
        }

        public async Task<EventResponseDto?> GetByIdAsync(int id)
        {
            var evt = await _context.Events
                .Include(e => e.Sport)
                .FirstOrDefaultAsync(e => e.Id == id);

            return evt == null ? null : MapToResponseDto(evt);
        }

        public async Task<EventResponseDto> CreateAsync(CreateEventDto dto)
        {
            var evt = new Event
            {
                Name = dto.Name,
                SportId = dto.SportId,
                EventType = dto.EventType,
                SortOrder = dto.SortOrder,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Events.Add(evt);
            await _context.SaveChangesAsync();

            // Reload with Sport navigation property
            await _context.Entry(evt).Reference(e => e.Sport).LoadAsync();

            return MapToResponseDto(evt);
        }

        public async Task<EventResponseDto?> UpdateAsync(int id, UpdateEventDto dto)
        {
            var evt = await _context.Events
                .Include(e => e.Sport)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (evt == null)
            {
                return null;
            }

            evt.Name = dto.Name;
            evt.SportId = dto.SportId;
            evt.EventType = dto.EventType;
            evt.SortOrder = dto.SortOrder;
            evt.IsActive = dto.IsActive;
            evt.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Reload Sport if SportId changed
            await _context.Entry(evt).Reference(e => e.Sport).LoadAsync();

            return MapToResponseDto(evt);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var evt = await _context.Events.FindAsync(id);
            if (evt == null)
            {
                return false;
            }

            // Soft delete
            evt.IsActive = false;
            evt.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        private static EventResponseDto MapToResponseDto(Event evt)
        {
            return new EventResponseDto
            {
                Id = evt.Id,
                Name = evt.Name ?? string.Empty,
                SportId = evt.SportId,
                SportName = evt.Sport?.Name ?? string.Empty,
                EventType = evt.EventType ?? string.Empty,
                SortOrder = evt.SortOrder,
                IsActive = evt.IsActive,
                CreatedAt = evt.CreatedAt,
                UpdatedAt = evt.UpdatedAt
            };
        }
    }
}
