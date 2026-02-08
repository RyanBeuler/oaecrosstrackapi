using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.DTOs.TeamMeetResults
{
    public class CreateTeamMeetResultDto
    {
        [Required]
        public int SportId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public DateTime MeetDate { get; set; }

        [Required]
        [StringLength(1)]
        public required string Gender { get; set; }

        [Required]
        [StringLength(100)]
        public required string HomeTeam { get; set; }

        [Required]
        [StringLength(100)]
        public required string AwayTeam { get; set; }

        [Required]
        public int HomeScore { get; set; }

        [Required]
        public int AwayScore { get; set; }

        public bool IsDivisionMatch { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class UpdateTeamMeetResultDto
    {
        [Required]
        public int SportId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public DateTime MeetDate { get; set; }

        [Required]
        [StringLength(1)]
        public required string Gender { get; set; }

        [Required]
        [StringLength(100)]
        public required string HomeTeam { get; set; }

        [Required]
        [StringLength(100)]
        public required string AwayTeam { get; set; }

        [Required]
        public int HomeScore { get; set; }

        [Required]
        public int AwayScore { get; set; }

        public bool IsDivisionMatch { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class TeamMeetResultResponseDto
    {
        public int Id { get; set; }
        public int SportId { get; set; }
        public required string SportName { get; set; }
        public int Year { get; set; }
        public DateTime MeetDate { get; set; }
        public required string Gender { get; set; }
        public required string HomeTeam { get; set; }
        public required string AwayTeam { get; set; }
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public bool IsDivisionMatch { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class TeamStandingEntryDto
    {
        public required string TeamName { get; set; }
        public int OverallWins { get; set; }
        public int OverallLosses { get; set; }
        public int DivisionWins { get; set; }
        public int DivisionLosses { get; set; }
    }

    public class StandingsResponseDto
    {
        public int SportId { get; set; }
        public required string SportName { get; set; }
        public int Year { get; set; }
        public required string Gender { get; set; }
        public List<TeamStandingEntryDto> Standings { get; set; } = new();
    }
}
