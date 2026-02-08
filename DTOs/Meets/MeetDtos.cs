using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.DTOs.Meets
{
    public class CreateMeetDto
    {
        [Required]
        public int SportId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [Required]
        public DateTime MeetDate { get; set; }

        [Required]
        [StringLength(20)]
        public required string MeetType { get; set; }

        [StringLength(100)]
        public string? Opponent { get; set; }

        public bool IsHome { get; set; } = true;

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class UpdateMeetDto
    {
        [Required]
        public int SportId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [Required]
        public DateTime MeetDate { get; set; }

        [Required]
        [StringLength(20)]
        public required string MeetType { get; set; }

        [StringLength(100)]
        public string? Opponent { get; set; }

        public bool IsHome { get; set; } = true;

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class MeetResponseDto
    {
        public int Id { get; set; }
        public int SportId { get; set; }
        public required string SportName { get; set; }
        public required string Name { get; set; }
        public string? Location { get; set; }
        public DateTime MeetDate { get; set; }
        public required string MeetType { get; set; }
        public string? Opponent { get; set; }
        public bool IsHome { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class MeetQueryDto
    {
        public int? SportId { get; set; }
        public int? Year { get; set; } // School year (filters by MeetDate)
        public bool? ActiveOnly { get; set; }
    }
}
