using Microsoft.EntityFrameworkCore;
using OaeCrosstrackApi.Data;
using OaeCrosstrackApi.DTOs.TeamMeetResults;
using OaeCrosstrackApi.Models;

namespace OaeCrosstrackApi.Services
{
    public interface ITeamMeetResultService
    {
        Task<IEnumerable<TeamMeetResultResponseDto>> GetAllAsync(int? sportId = null, int? year = null, string? gender = null, bool activeOnly = true);
        Task<TeamMeetResultResponseDto?> GetByIdAsync(int id);
        Task<TeamMeetResultResponseDto> CreateAsync(CreateTeamMeetResultDto dto);
        Task<TeamMeetResultResponseDto?> UpdateAsync(int id, UpdateTeamMeetResultDto dto);
        Task<bool> DeleteAsync(int id);
        Task<StandingsResponseDto> GetStandingsAsync(int sportId, int year, string gender);
        Task<IEnumerable<string>> GetDistinctTeamsAsync(int sportId, int year);
        Task<IEnumerable<int>> GetAvailableYearsAsync();
    }

    public class TeamMeetResultService : ITeamMeetResultService
    {
        private readonly ApplicationDbContext _context;

        public TeamMeetResultService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TeamMeetResultResponseDto>> GetAllAsync(int? sportId = null, int? year = null, string? gender = null, bool activeOnly = true)
        {
            var query = _context.TeamMeetResults
                .Include(t => t.Sport)
                .AsQueryable();

            if (activeOnly)
            {
                query = query.Where(t => t.IsActive);
            }

            if (sportId.HasValue)
            {
                query = query.Where(t => t.SportId == sportId.Value);
            }

            if (year.HasValue)
            {
                query = query.Where(t => t.Year == year.Value);
            }

            if (!string.IsNullOrEmpty(gender))
            {
                query = query.Where(t => t.Gender == gender);
            }

            var results = await query
                .OrderByDescending(t => t.MeetDate)
                .ToListAsync();

            return results.Select(MapToResponseDto);
        }

        public async Task<TeamMeetResultResponseDto?> GetByIdAsync(int id)
        {
            var result = await _context.TeamMeetResults
                .Include(t => t.Sport)
                .FirstOrDefaultAsync(t => t.Id == id);

            return result == null ? null : MapToResponseDto(result);
        }

        public async Task<TeamMeetResultResponseDto> CreateAsync(CreateTeamMeetResultDto dto)
        {
            var result = new TeamMeetResult
            {
                SportId = dto.SportId,
                Year = dto.Year,
                MeetDate = dto.MeetDate,
                Gender = dto.Gender,
                HomeTeam = dto.HomeTeam,
                AwayTeam = dto.AwayTeam,
                HomeScore = dto.HomeScore,
                AwayScore = dto.AwayScore,
                IsDivisionMatch = dto.IsDivisionMatch,
                Notes = dto.Notes,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.TeamMeetResults.Add(result);
            await _context.SaveChangesAsync();

            await _context.Entry(result).Reference(t => t.Sport).LoadAsync();

            return MapToResponseDto(result);
        }

        public async Task<TeamMeetResultResponseDto?> UpdateAsync(int id, UpdateTeamMeetResultDto dto)
        {
            var result = await _context.TeamMeetResults
                .Include(t => t.Sport)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (result == null)
            {
                return null;
            }

            result.SportId = dto.SportId;
            result.Year = dto.Year;
            result.MeetDate = dto.MeetDate;
            result.Gender = dto.Gender;
            result.HomeTeam = dto.HomeTeam;
            result.AwayTeam = dto.AwayTeam;
            result.HomeScore = dto.HomeScore;
            result.AwayScore = dto.AwayScore;
            result.IsDivisionMatch = dto.IsDivisionMatch;
            result.Notes = dto.Notes;
            result.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            await _context.Entry(result).Reference(t => t.Sport).LoadAsync();

            return MapToResponseDto(result);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _context.TeamMeetResults.FindAsync(id);
            if (result == null)
            {
                return false;
            }

            result.IsActive = false;
            result.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<StandingsResponseDto> GetStandingsAsync(int sportId, int year, string gender)
        {
            var results = await _context.TeamMeetResults
                .Include(t => t.Sport)
                .Where(t => t.IsActive && t.SportId == sportId && t.Year == year && t.Gender == gender)
                .ToListAsync();

            var sportName = results.FirstOrDefault()?.Sport?.Name ?? string.Empty;
            if (string.IsNullOrEmpty(sportName))
            {
                var sport = await _context.Sports.FindAsync(sportId);
                sportName = sport?.Name ?? string.Empty;
            }

            var teamStats = new Dictionary<string, TeamStandingEntryDto>();

            foreach (var r in results)
            {
                // Process home team
                if (!teamStats.ContainsKey(r.HomeTeam))
                {
                    teamStats[r.HomeTeam] = new TeamStandingEntryDto { TeamName = r.HomeTeam };
                }
                var homeEntry = teamStats[r.HomeTeam];
                if (r.HomeScore > r.AwayScore)
                {
                    homeEntry.OverallWins++;
                    if (r.IsDivisionMatch) homeEntry.DivisionWins++;
                }
                else if (r.HomeScore < r.AwayScore)
                {
                    homeEntry.OverallLosses++;
                    if (r.IsDivisionMatch) homeEntry.DivisionLosses++;
                }

                // Process away team
                if (!teamStats.ContainsKey(r.AwayTeam))
                {
                    teamStats[r.AwayTeam] = new TeamStandingEntryDto { TeamName = r.AwayTeam };
                }
                var awayEntry = teamStats[r.AwayTeam];
                if (r.AwayScore > r.HomeScore)
                {
                    awayEntry.OverallWins++;
                    if (r.IsDivisionMatch) awayEntry.DivisionWins++;
                }
                else if (r.AwayScore < r.HomeScore)
                {
                    awayEntry.OverallLosses++;
                    if (r.IsDivisionMatch) awayEntry.DivisionLosses++;
                }
            }

            var standings = teamStats.Values
                .OrderByDescending(s => s.OverallWins)
                .ThenBy(s => s.OverallLosses)
                .ToList();

            return new StandingsResponseDto
            {
                SportId = sportId,
                SportName = sportName,
                Year = year,
                Gender = gender,
                Standings = standings
            };
        }

        public async Task<IEnumerable<string>> GetDistinctTeamsAsync(int sportId, int year)
        {
            var homeTeams = await _context.TeamMeetResults
                .Where(t => t.IsActive && t.SportId == sportId && t.Year == year)
                .Select(t => t.HomeTeam)
                .Distinct()
                .ToListAsync();

            var awayTeams = await _context.TeamMeetResults
                .Where(t => t.IsActive && t.SportId == sportId && t.Year == year)
                .Select(t => t.AwayTeam)
                .Distinct()
                .ToListAsync();

            return homeTeams.Union(awayTeams).Distinct().OrderBy(t => t);
        }

        public async Task<IEnumerable<int>> GetAvailableYearsAsync()
        {
            var years = await _context.TeamMeetResults
                .Where(t => t.IsActive)
                .Select(t => t.Year)
                .Distinct()
                .OrderByDescending(y => y)
                .ToListAsync();

            var currentYear = DateTime.Now.Month >= 8 ? DateTime.Now.Year + 1 : DateTime.Now.Year;
            if (!years.Contains(currentYear))
            {
                years.Insert(0, currentYear);
            }

            return years.OrderByDescending(y => y);
        }

        private static TeamMeetResultResponseDto MapToResponseDto(TeamMeetResult result)
        {
            return new TeamMeetResultResponseDto
            {
                Id = result.Id,
                SportId = result.SportId,
                SportName = result.Sport?.Name ?? string.Empty,
                Year = result.Year,
                MeetDate = result.MeetDate,
                Gender = result.Gender,
                HomeTeam = result.HomeTeam,
                AwayTeam = result.AwayTeam,
                HomeScore = result.HomeScore,
                AwayScore = result.AwayScore,
                IsDivisionMatch = result.IsDivisionMatch,
                Notes = result.Notes,
                IsActive = result.IsActive,
                CreatedAt = result.CreatedAt,
                UpdatedAt = result.UpdatedAt
            };
        }
    }
}
