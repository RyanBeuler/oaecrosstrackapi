using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.DTOs.Records
{
    public class CreateRecordDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int AthleteId { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Performance { get; set; } = string.Empty;

        [Required]
        public decimal PerformanceValue { get; set; }

        [StringLength(10)]
        public string? GradeAtTime { get; set; }

        public DateTime PerformanceDate { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(200)]
        public string? MeetName { get; set; }

        [Required]
        [StringLength(50)]
        public string RecordType { get; set; } = "School";
    }

    public class UpdateRecordDto
    {
        [Required]
        public int EventId { get; set; }

        [Required]
        public int AthleteId { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Performance { get; set; } = string.Empty;

        [Required]
        public decimal PerformanceValue { get; set; }

        [StringLength(10)]
        public string? GradeAtTime { get; set; }

        public DateTime PerformanceDate { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(200)]
        public string? MeetName { get; set; }

        [Required]
        [StringLength(50)]
        public string RecordType { get; set; } = "School";
    }

    public class RecordResponseDto
    {
        public int Id { get; set; }
        public int EventId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public int SportId { get; set; }
        public string SportName { get; set; } = string.Empty;
        public int AthleteId { get; set; }
        public string AthleteName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Performance { get; set; } = string.Empty;
        public decimal PerformanceValue { get; set; }
        public string? GradeAtTime { get; set; }
        public DateTime PerformanceDate { get; set; }
        public string? Location { get; set; }
        public string? MeetName { get; set; }
        public string RecordType { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class RecordQueryDto
    {
        public int? EventId { get; set; }
        public int? SportId { get; set; }
        public int? AthleteId { get; set; }
        public string? Gender { get; set; }
        public string? RecordType { get; set; }
        public bool ActiveOnly { get; set; } = true;
    }
}
