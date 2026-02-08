using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OaeCrosstrackApi.Models
{
    public class Result
    {
        [Key]
        public int Id { get; set; }

        // Nullable for relay events (relay results have no individual athlete)
        public int? AthleteId { get; set; }

        [ForeignKey("AthleteId")]
        public Athlete? Athlete { get; set; }

        [Required]
        public int MeetId { get; set; }

        [ForeignKey("MeetId")]
        public Meet? Meet { get; set; }

        [Required]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public Event? Event { get; set; }

        // For relay events: team name (e.g., "Williamson Central 'A'")
        [StringLength(100)]
        public string? RelayTeamName { get; set; }

        // Heat number for track events
        public int? HeatNumber { get; set; }

        // Result status: null = normal, "DQ", "DNS", "DNF", "SCR"
        [StringLength(10)]
        public string? ResultStatus { get; set; }

        // Wind speed in m/s (outdoor sprints/jumps)
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Wind { get; set; }

        // Performance stored as decimal (seconds for time, inches/cm for distance/height)
        [Column(TypeName = "decimal(10,3)")]
        public decimal? Performance { get; set; }

        // Display format for the performance (e.g., "5:23.45", "18'6\"", "5.50m")
        [StringLength(20)]
        public string? PerformanceDisplay { get; set; }

        public int? Place { get; set; }

        public int? Points { get; set; }

        public bool IsPR { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
