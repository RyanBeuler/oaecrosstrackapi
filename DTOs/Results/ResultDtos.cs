using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.DTOs.Results
{
    public class CreateResultDto
    {
        [Required]
        public int AthleteId { get; set; }

        [Required]
        public int MeetId { get; set; }

        [Required]
        public int EventId { get; set; }

        public decimal? Performance { get; set; }

        [StringLength(20)]
        public string? PerformanceDisplay { get; set; }

        public int? Place { get; set; }

        public int? Points { get; set; }

        public bool IsPR { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class UpdateResultDto
    {
        [Required]
        public int AthleteId { get; set; }

        [Required]
        public int MeetId { get; set; }

        [Required]
        public int EventId { get; set; }

        public decimal? Performance { get; set; }

        [StringLength(20)]
        public string? PerformanceDisplay { get; set; }

        public int? Place { get; set; }

        public int? Points { get; set; }

        public bool IsPR { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class ResultResponseDto
    {
        public int Id { get; set; }
        public int AthleteId { get; set; }
        public required string AthleteFirstName { get; set; }
        public required string AthleteLastName { get; set; }
        public required string AthleteGradeLevel { get; set; }
        public int MeetId { get; set; }
        public required string MeetName { get; set; }
        public DateTime MeetDate { get; set; }
        public int EventId { get; set; }
        public required string EventName { get; set; }
        public required string EventType { get; set; }
        public decimal? Performance { get; set; }
        public string? PerformanceDisplay { get; set; }
        public int? Place { get; set; }
        public int? Points { get; set; }
        public bool IsPR { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ResultQueryDto
    {
        public int? AthleteId { get; set; }
        public int? MeetId { get; set; }
        public int? EventId { get; set; }
        public int? SportId { get; set; }
        public bool? ActiveOnly { get; set; }
    }

    public class BulkCreateResultDto
    {
        [Required]
        public int MeetId { get; set; }

        [Required]
        public int EventId { get; set; }

        [Required]
        public required List<AthleteResultDto> Results { get; set; }
    }

    public class AthleteResultDto
    {
        [Required]
        public int AthleteId { get; set; }

        public decimal? Performance { get; set; }

        [StringLength(20)]
        public string? PerformanceDisplay { get; set; }

        public int? Place { get; set; }

        public int? Points { get; set; }

        public bool IsPR { get; set; } = false;

        [StringLength(500)]
        public string? Notes { get; set; }
    }
}
