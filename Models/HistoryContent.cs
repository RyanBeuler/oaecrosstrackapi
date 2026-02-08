using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OaeCrosstrackApi.Models
{
    public class HistoryContent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SportId { get; set; }

        [ForeignKey("SportId")]
        public Sport? Sport { get; set; }

        public string MarkdownContent { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
