using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OaeCrosstrackApi.Models
{
    public class Record
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

        [ForeignKey("EventId")]
        public Event? Event { get; set; }

        [Required]
        public int AthleteId { get; set; }

        [ForeignKey("AthleteId")]
        public Athlete? Athlete { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Performance { get; set; } = string.Empty; // Display format: "11.2", "5'10\"", "18:45.2"

        [Required]
        [Column(TypeName = "decimal(18,4)")]
        public decimal PerformanceValue { get; set; } // Numeric value for sorting (lower is better for time, higher for distance/height)

        [StringLength(10)]
        public string? GradeAtTime { get; set; } // Fr, So, Jr, Sr

        public DateTime PerformanceDate { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(200)]
        public string? MeetName { get; set; }

        [Required]
        [StringLength(50)]
        public string RecordType { get; set; } = "School"; // School, Conference, State

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
