using Microsoft.EntityFrameworkCore;
using OaeCrosstrackApi.Data;
using OaeCrosstrackApi.DTOs.Rosters;
using OaeCrosstrackApi.Models;

namespace OaeCrosstrackApi.Services
{
    public interface IRosterService
    {
        Task<IEnumerable<RosterEntryResponseDto>> GetAllAsync(int? sportId = null, int? year = null);
        Task<RosterEntryResponseDto?> GetByIdAsync(int id);
        Task<RosterEntryResponseDto> CreateAsync(CreateRosterEntryDto dto);
        Task<IEnumerable<RosterEntryResponseDto>> BulkAddAsync(BulkAddRosterEntriesDto dto);
        Task<bool> DeleteAsync(int id);
        Task<int> BulkDeleteAsync(int sportId, int year, int[] athleteIds);
        Task<IEnumerable<int>> GetAvailableYearsAsync();
    }

    public class RosterService : IRosterService
    {
        private readonly ApplicationDbContext _context;

        public RosterService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<RosterEntryResponseDto>> GetAllAsync(int? sportId = null, int? year = null)
        {
            var query = _context.RosterEntries
                .Include(r => r.Athlete)
                .Include(r => r.Sport)
                .Where(r => r.Athlete != null && r.Athlete.IsActive)
                .AsQueryable();

            if (sportId.HasValue)
            {
                query = query.Where(r => r.SportId == sportId.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(r => r.Year == year.Value);
            }

            var entries = await query
                .OrderBy(r => r.Athlete!.LastName)
                .ThenBy(r => r.Athlete!.FirstName)
                .ToListAsync();

            return entries.Select(MapToResponseDto);
        }

        public async Task<RosterEntryResponseDto?> GetByIdAsync(int id)
        {
            var entry = await _context.RosterEntries
                .Include(r => r.Athlete)
                .Include(r => r.Sport)
                .FirstOrDefaultAsync(r => r.Id == id);

            return entry == null ? null : MapToResponseDto(entry);
        }

        public async Task<RosterEntryResponseDto> CreateAsync(CreateRosterEntryDto dto)
        {
            // Check if entry already exists
            var exists = await _context.RosterEntries
                .AnyAsync(r => r.AthleteId == dto.AthleteId && r.SportId == dto.SportId && r.Year == dto.Year);

            if (exists)
            {
                throw new InvalidOperationException("Athlete is already on this roster for the specified year.");
            }

            var entry = new RosterEntry
            {
                AthleteId = dto.AthleteId,
                SportId = dto.SportId,
                Year = dto.Year,
                CreatedAt = DateTime.UtcNow
            };

            _context.RosterEntries.Add(entry);
            await _context.SaveChangesAsync();

            // Reload with navigation properties
            await _context.Entry(entry).Reference(r => r.Athlete).LoadAsync();
            await _context.Entry(entry).Reference(r => r.Sport).LoadAsync();

            return MapToResponseDto(entry);
        }

        public async Task<IEnumerable<RosterEntryResponseDto>> BulkAddAsync(BulkAddRosterEntriesDto dto)
        {
            var results = new List<RosterEntryResponseDto>();

            foreach (var athleteId in dto.AthleteIds)
            {
                // Check if entry already exists
                var exists = await _context.RosterEntries
                    .AnyAsync(r => r.AthleteId == athleteId && r.SportId == dto.SportId && r.Year == dto.Year);

                if (!exists)
                {
                    var entry = new RosterEntry
                    {
                        AthleteId = athleteId,
                        SportId = dto.SportId,
                        Year = dto.Year,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.RosterEntries.Add(entry);
                }
            }

            await _context.SaveChangesAsync();

            // Return the current roster for this sport/year
            return await GetAllAsync(dto.SportId, dto.Year);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entry = await _context.RosterEntries.FindAsync(id);
            if (entry == null)
            {
                return false;
            }

            _context.RosterEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> BulkDeleteAsync(int sportId, int year, int[] athleteIds)
        {
            var entries = await _context.RosterEntries
                .Where(r => r.SportId == sportId && r.Year == year && athleteIds.Contains(r.AthleteId))
                .ToListAsync();

            _context.RosterEntries.RemoveRange(entries);
            await _context.SaveChangesAsync();

            return entries.Count;
        }

        public async Task<IEnumerable<int>> GetAvailableYearsAsync()
        {
            var years = await _context.RosterEntries
                .Select(r => r.Year)
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

        private static RosterEntryResponseDto MapToResponseDto(RosterEntry entry)
        {
            return new RosterEntryResponseDto
            {
                Id = entry.Id,
                AthleteId = entry.AthleteId,
                AthleteFirstName = entry.Athlete?.FirstName ?? string.Empty,
                AthleteLastName = entry.Athlete?.LastName ?? string.Empty,
                AthleteGradeLevel = CalculateGradeLevel(entry.Athlete?.GraduationYear ?? 0),
                AthleteGender = entry.Athlete?.Gender ?? string.Empty,
                SportId = entry.SportId,
                SportName = entry.Sport?.Name ?? string.Empty,
                Year = entry.Year,
                CreatedAt = entry.CreatedAt
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
