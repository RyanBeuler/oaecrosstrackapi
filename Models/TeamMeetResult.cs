using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OaeCrosstrackApi.Models
{
    public class TeamMeetResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SportId { get; set; }

        [ForeignKey("SportId")]
        public Sport? Sport { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public DateTime MeetDate { get; set; }

        [Required]
        [StringLength(1)]
        public required string Gender { get; set; } // "M" or "F"

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

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
