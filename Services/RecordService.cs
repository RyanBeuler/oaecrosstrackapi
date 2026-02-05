using Microsoft.EntityFrameworkCore;
using OaeCrosstrackApi.Data;
using OaeCrosstrackApi.DTOs.Records;
using OaeCrosstrackApi.Models;

namespace OaeCrosstrackApi.Services
{
    public interface IRecordService
    {
        Task<IEnumerable<RecordResponseDto>> GetAllAsync(RecordQueryDto? query = null);
        Task<RecordResponseDto?> GetByIdAsync(int id);
        Task<IEnumerable<RecordResponseDto>> GetByEventAsync(int eventId);
        Task<IEnumerable<RecordResponseDto>> GetBySportAsync(int sportId);
        Task<IEnumerable<RecordResponseDto>> GetLeaderboardAsync(int eventId, string gender, int top = 10);
        Task<RecordResponseDto> CreateAsync(CreateRecordDto dto);
        Task<RecordResponseDto?> UpdateAsync(int id, UpdateRecordDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public class RecordService : IRecordService
    {
        private readonly ApplicationDbContext _context;

        public RecordService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RecordResponseDto>> GetAllAsync(RecordQueryDto? query = null)
        {
            var queryable = _context.Records
                .Include(r => r.Event)
                    .ThenInclude(e => e!.Sport)
                .Include(r => r.Athlete)
                .AsQueryable();

            if (query != null)
            {
                if (query.ActiveOnly)
                {
                    queryable = queryable.Where(r => r.IsActive);
                }

                if (query.EventId.HasValue)
                {
                    queryable = queryable.Where(r => r.EventId == query.EventId.Value);
                }

                if (query.SportId.HasValue)
                {
                    queryable = queryable.Where(r => r.Event != null && r.Event.SportId == query.SportId.Value);
                }

                if (query.AthleteId.HasValue)
                {
                    queryable = queryable.Where(r => r.AthleteId == query.AthleteId.Value);
                }

                if (!string.IsNullOrEmpty(query.Gender))
                {
                    queryable = queryable.Where(r => r.Gender == query.Gender);
                }

                if (!string.IsNullOrEmpty(query.RecordType))
                {
                    queryable = queryable.Where(r => r.RecordType == query.RecordType);
                }
            }

            var records = await queryable
                .OrderBy(r => r.Event!.Name)
                .ThenBy(r => r.PerformanceValue)
                .ToListAsync();

            return records.Select(MapToResponseDto);
        }

        public async Task<RecordResponseDto?> GetByIdAsync(int id)
        {
            var record = await _context.Records
                .Include(r => r.Event)
                    .ThenInclude(e => e!.Sport)
                .Include(r => r.Athlete)
                .FirstOrDefaultAsync(r => r.Id == id);

            return record == null ? null : MapToResponseDto(record);
        }

        public async Task<IEnumerable<RecordResponseDto>> GetByEventAsync(int eventId)
        {
            var records = await _context.Records
                .Include(r => r.Event)
                    .ThenInclude(e => e!.Sport)
                .Include(r => r.Athlete)
                .Where(r => r.EventId == eventId && r.IsActive)
                .OrderBy(r => r.PerformanceValue)
                .ToListAsync();

            return records.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<RecordResponseDto>> GetBySportAsync(int sportId)
        {
            var records = await _context.Records
                .Include(r => r.Event)
                    .ThenInclude(e => e!.Sport)
                .Include(r => r.Athlete)
                .Where(r => r.Event != null && r.Event.SportId == sportId && r.IsActive)
                .OrderBy(r => r.Event!.Name)
                .ThenBy(r => r.PerformanceValue)
                .ToListAsync();

            return records.Select(MapToResponseDto);
        }

        public async Task<IEnumerable<RecordResponseDto>> GetLeaderboardAsync(int eventId, string gender, int top = 10)
        {
            var records = await _context.Records
                .Include(r => r.Event)
                    .ThenInclude(e => e!.Sport)
                .Include(r => r.Athlete)
                .Where(r => r.EventId == eventId && r.Gender == gender && r.IsActive)
                .OrderBy(r => r.PerformanceValue)
                .Take(top)
                .ToListAsync();

            return records.Select(MapToResponseDto);
        }

        public async Task<RecordResponseDto> CreateAsync(CreateRecordDto dto)
        {
            var record = new Record
            {
                EventId = dto.EventId,
                AthleteId = dto.AthleteId,
                Gender = dto.Gender,
                Performance = dto.Performance,
                PerformanceValue = dto.PerformanceValue,
                GradeAtTime = dto.GradeAtTime,
                PerformanceDate = DateTime.SpecifyKind(dto.PerformanceDate, DateTimeKind.Utc),
                Location = dto.Location,
                MeetName = dto.MeetName,
                RecordType = dto.RecordType,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Records.Add(record);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            var created = await _context.Records
                .Include(r => r.Event)
                    .ThenInclude(e => e!.Sport)
                .Include(r => r.Athlete)
                .FirstAsync(r => r.Id == record.Id);

            return MapToResponseDto(created);
        }

        public async Task<RecordResponseDto?> UpdateAsync(int id, UpdateRecordDto dto)
        {
            var record = await _context.Records
                .Include(r => r.Event)
                    .ThenInclude(e => e!.Sport)
                .Include(r => r.Athlete)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (record == null)
            {
                return null;
            }

            record.EventId = dto.EventId;
            record.AthleteId = dto.AthleteId;
            record.Gender = dto.Gender;
            record.Performance = dto.Performance;
            record.PerformanceValue = dto.PerformanceValue;
            record.GradeAtTime = dto.GradeAtTime;
            record.PerformanceDate = DateTime.SpecifyKind(dto.PerformanceDate, DateTimeKind.Utc);
            record.Location = dto.Location;
            record.MeetName = dto.MeetName;
            record.RecordType = dto.RecordType;
            record.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Reload to get updated navigation properties
            var updated = await _context.Records
                .Include(r => r.Event)
                    .ThenInclude(e => e!.Sport)
                .Include(r => r.Athlete)
                .FirstAsync(r => r.Id == id);

            return MapToResponseDto(updated);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var record = await _context.Records.FindAsync(id);

            if (record == null)
            {
                return false;
            }

            // Soft delete
            record.IsActive = false;
            record.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        private static RecordResponseDto MapToResponseDto(Record record)
        {
            return new RecordResponseDto
            {
                Id = record.Id,
                EventId = record.EventId,
                EventName = record.Event?.Name ?? string.Empty,
                SportId = record.Event?.SportId ?? 0,
                SportName = record.Event?.Sport?.Name ?? string.Empty,
                AthleteId = record.AthleteId,
                AthleteName = record.Athlete != null
                    ? $"{record.Athlete.FirstName} {record.Athlete.LastName}"
                    : string.Empty,
                Gender = record.Gender,
                Performance = record.Performance,
                PerformanceValue = record.PerformanceValue,
                GradeAtTime = record.GradeAtTime,
                PerformanceDate = record.PerformanceDate,
                Location = record.Location,
                MeetName = record.MeetName,
                RecordType = record.RecordType,
                IsActive = record.IsActive,
                CreatedAt = record.CreatedAt,
                UpdatedAt = record.UpdatedAt
            };
        }
    }
}
