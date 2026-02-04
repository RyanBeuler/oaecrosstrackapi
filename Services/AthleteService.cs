using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OaeCrosstrackApi.Data;
using OaeCrosstrackApi.DTOs.Athletes;
using OaeCrosstrackApi.Models;

namespace OaeCrosstrackApi.Services
{
    public interface IAthleteService
    {
        Task<IEnumerable<AthleteResponseDto>> GetAllAsync(bool includeInactive = false);
        Task<AthleteResponseDto?> GetByIdAsync(int id);
        Task<AthleteResponseDto> CreateAsync(CreateAthleteDto dto);
        Task<AthleteResponseDto?> UpdateAsync(int id, UpdateAthleteDto dto);
        Task<bool> DeleteAsync(int id);
    }

    public class AthleteService : IAthleteService
    {
        private readonly ApplicationDbContext _context;

        public AthleteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AthleteResponseDto>> GetAllAsync(bool includeInactive = false)
        {
            var query = _context.Athletes.AsQueryable();

            if (!includeInactive)
            {
                query = query.Where(a => a.IsActive);
            }

            var athletes = await query
                .OrderBy(a => a.LastName)
                .ThenBy(a => a.FirstName)
                .ToListAsync();

            return athletes.Select(MapToResponseDto);
        }

        public async Task<AthleteResponseDto?> GetByIdAsync(int id)
        {
            var athlete = await _context.Athletes.FindAsync(id);
            return athlete == null ? null : MapToResponseDto(athlete);
        }

        public async Task<AthleteResponseDto> CreateAsync(CreateAthleteDto dto)
        {
            var athlete = new Athlete
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                GraduationYear = dto.GraduationYear,
                Gender = dto.Gender,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.Athletes.Add(athlete);
            await _context.SaveChangesAsync();

            return MapToResponseDto(athlete);
        }

        public async Task<AthleteResponseDto?> UpdateAsync(int id, UpdateAthleteDto dto)
        {
            var athlete = await _context.Athletes.FindAsync(id);
            if (athlete == null)
            {
                return null;
            }

            athlete.FirstName = dto.FirstName;
            athlete.LastName = dto.LastName;
            athlete.GraduationYear = dto.GraduationYear;
            athlete.Gender = dto.Gender;
            athlete.IsActive = dto.IsActive;
            athlete.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToResponseDto(athlete);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var athlete = await _context.Athletes.FindAsync(id);
            if (athlete == null)
            {
                return false;
            }

            // Soft delete - set IsActive to false
            athlete.IsActive = false;
            athlete.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        private static AthleteResponseDto MapToResponseDto(Athlete athlete)
        {
            return new AthleteResponseDto
            {
                Id = athlete.Id,
                FirstName = athlete.FirstName ?? string.Empty,
                LastName = athlete.LastName ?? string.Empty,
                GraduationYear = athlete.GraduationYear,
                Gender = athlete.Gender ?? string.Empty,
                IsActive = athlete.IsActive,
                GradeLevel = CalculateGradeLevel(athlete.GraduationYear),
                CreatedAt = athlete.CreatedAt,
                UpdatedAt = athlete.UpdatedAt
            };
        }

        private static string CalculateGradeLevel(int graduationYear)
        {
            // Calculate based on school year (August start)
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
