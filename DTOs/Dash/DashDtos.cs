using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.DTOs.Dash
{
    public class UpsertDashDto
    {
        public string RegistrationMarkdown { get; set; } = string.Empty;
        public string PastResultsMarkdown { get; set; } = string.Empty;
        public string CourseMapMarkdown { get; set; } = string.Empty;
    }

    public class DashResponseDto
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string RegistrationMarkdown { get; set; } = string.Empty;
        public string PastResultsMarkdown { get; set; } = string.Empty;
        public string CourseMapMarkdown { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<DashFileResponseDto> Files { get; set; } = new();
    }

    public class DashFileResponseDto
    {
        public int Id { get; set; }
        public int DashContentId { get; set; }
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
