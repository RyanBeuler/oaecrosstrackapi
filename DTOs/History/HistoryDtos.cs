using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.DTOs.History
{
    public class UpsertHistoryDto
    {
        [Required]
        public string MarkdownContent { get; set; } = string.Empty;
    }

    public class HistoryResponseDto
    {
        public int Id { get; set; }
        public int SportId { get; set; }
        public string SportName { get; set; } = string.Empty;
        public string MarkdownContent { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
