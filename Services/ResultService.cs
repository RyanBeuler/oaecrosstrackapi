using Microsoft.EntityFrameworkCore;
using OaeCrosstrackApi.Data;
using OaeCrosstrackApi.DTOs.Results;
using OaeCrosstrackApi.Models;

namespace OaeCrosstrackApi.Services
{
    public interface IResultService
    {
        Task<IEnumerable<ResultResponseDto>> GetAllAsync(int? athleteId = null, int? meetId = null, int? eventId = null, int? sportId = null, bool activeOnly = true);
        Task<ResultResponseDto?> GetByIdAsync(int id);
        Task<ResultResponseDto> CreateAsync(CreateResultDto dto);
        Task<IEnumerable<ResultResponseDto>> BulkCreateAsync(BulkCreateResultDto dto);
        Task<ResultResponseDto?> UpdateAsync(int id, UpdateResultDto dto);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<ResultResponseDto>> GetByAthleteAsync(int athleteId);
        Task<IEnumerable<ResultResponseDto>> GetByMeetAsync(int meetId);
    }

    public class ResultService : IResultService
    {
        private readonly ApplicationDbContext _context;

        public ResultService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ResultResponseDto>> GetAllAsync(int? athleteId = null, int? meetId = null, int? eventId = null, int? sportId = null, bool activeOnly = true)
        {
            var query = _context.Results
                .Include(r => r.Athlete)
                .Include(r => r.Meet)
                .Include(r => r.Event)
                .AsQueryable();

            if (activeOnly)
            {
                query = query.Where(r => r.IsActive);
            }

            if (athleteId.HasValue)
            {
                query = query.Where(r => r.AthleteId == athleteId.Value);
            }

            if (meetId.HasValue)
            {
                query = query.Where(r => r.MeetId == meetId.Value);
            }

            if (eventId.HasValue)
            {
                query = query.Where(r => r.EventId == eventId.Value);
            }

            if (sportId.HasValue)
            {
                query = query.Where(r => r.Event != null && r.Event.SportId == sportId.Value);
            }

            var results = await query
                .OrderBy(r => r.Meet!.MeetDate)
                .ThenBy(r => r.Event!.Name)
                .ThenBy(r => r.Place)
                .ToListAsync();

            return results.Select(MapToResponseDto);
        }

        public async Task<ResultResponseDto?> GetByIdAsync(int id)
        {
            var result = await _context.Results
                .Include(r => r.Athlete)
                .Include(r => r.Meet)
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.Id == id);

            return result == null ? null : MapToResponseDto(result);
        }

        public async Task<ResultResponseDto> CreateAsync(CreateResultDto dto)
        {
            var result = new Result
            {
                AthleteId = dto.AthleteId,
                MeetId = dto.MeetId,
                EventId = dto.EventId,
                Performance = dto.Performance,
                PerformanceDisplay = dto.PerformanceDisplay,
                Place = dto.Place,
                Points = dto.Points,
                IsPR = dto.IsPR,
                Notes = dto.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            await _context.Entry(result).Reference(r => r.Athlete).LoadAsync();
            await _context.Entry(result).Reference(r => r.Meet).LoadAsync();
            await _context.Entry(result).Reference(r => r.Event).LoadAsync();

            return MapToResponseDto(result);
        }

        public async Task<IEnumerable<ResultResponseDto>> BulkCreateAsync(BulkCreateResultDto dto)
        {
            var createdResults = new List<Result>();

            foreach (var athleteResult in dto.Results)
            {
                // Check if result already exists for this athlete/meet/event
                var exists = await _context.Results
                    .AnyAsync(r => r.AthleteId == athleteResult.AthleteId
                        && r.MeetId == dto.MeetId
                        && r.EventId == dto.EventId
                        && r.IsActive);

                if (!exists)
                {
                    var result = new Result
                    {
                        AthleteId = athleteResult.AthleteId,
                        MeetId = dto.MeetId,
                        EventId = dto.EventId,
                        Performance = athleteResult.Performance,
                        PerformanceDisplay = athleteResult.PerformanceDisplay,
                        Place = athleteResult.Place,
                        Points = athleteResult.Points,
                        IsPR = athleteResult.IsPR,
                        Notes = athleteResult.Notes,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    _context.Results.Add(result);
                    createdResults.Add(result);
                }
            }

            await _context.SaveChangesAsync();

            // Return all results for this meet/event
            return await GetAllAsync(meetId: dto.MeetId, eventId: dto.EventId);
        }

        public async Task<ResultResponseDto?> UpdateAsync(int id, UpdateResultDto dto)
        {
            var result = await _context.Results
                .Include(r => r.Athlete)
                .Include(r => r.Meet)
                .Include(r => r.Event)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result == null)
            {
                return null;
            }

            result.AthleteId = dto.AthleteId;
            result.MeetId = dto.MeetId;
            result.EventId = dto.EventId;
            result.Performance = dto.Performance;
            result.PerformanceDisplay = dto.PerformanceDisplay;
            result.Place = dto.Place;
            result.Points = dto.Points;
            result.IsPR = dto.IsPR;
            result.Notes = dto.Notes;
            result.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Reload navigation properties if they changed
            await _context.Entry(result).Reference(r => r.Athlete).LoadAsync();
            await _context.Entry(result).Reference(r => r.Meet).LoadAsync();
            await _context.Entry(result).Reference(r => r.Event).LoadAsync();

            return MapToResponseDto(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _context.Results.FindAsync(id);
            if (result == null)
            {
                return false;
            }

            // Soft delete
            result.IsActive = false;
            result.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<ResultResponseDto>> GetByAthleteAsync(int athleteId)
        {
            return await GetAllAsync(athleteId: athleteId);
        }

        public async Task<IEnumerable<ResultResponseDto>> GetByMeetAsync(int meetId)
        {
            return await GetAllAsync(meetId: meetId);
        }

        private static ResultResponseDto MapToResponseDto(Result result)
        {
            return new ResultResponseDto
            {
                Id = result.Id,
                AthleteId = result.AthleteId,
                AthleteFirstName = result.Athlete?.FirstName ?? string.Empty,
                AthleteLastName = result.Athlete?.LastName ?? string.Empty,
                AthleteGradeLevel = CalculateGradeLevel(result.Athlete?.GraduationYear ?? 0),
                MeetId = result.MeetId,
                MeetName = result.Meet?.Name ?? string.Empty,
                MeetDate = result.Meet?.MeetDate ?? DateTime.MinValue,
                EventId = result.EventId,
                EventName = result.Event?.Name ?? string.Empty,
                EventType = result.Event?.EventType ?? string.Empty,
                Performance = result.Performance,
                PerformanceDisplay = result.PerformanceDisplay,
                Place = result.Place,
                Points = result.Points,
                IsPR = result.IsPR,
                Notes = result.Notes,
                IsActive = result.IsActive,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt
            };
        }

        private static string CalculateGradeLevel(int graduationYear)
        {
            var today = DateTime.Today;
            var currentSchoolYear = today.Month >= 8 ? today.Year + 1 : today.Year;
            var yearsUntilGraduation = graduationYear - currentSchoolYear;

            return yearsUntilGraduation switch
            {
                0 => "Senior",
                1 => "Junior",
                2 => "Sophomore",
                3 => "Freshman",
                < 0 => "Alumni",
                _ => "Future"
            };
        }
    }
}
