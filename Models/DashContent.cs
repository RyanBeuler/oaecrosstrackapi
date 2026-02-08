using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.Models
{
    public class DashContent
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int Year { get; set; }

        public string RegistrationMarkdown { get; set; } = string.Empty;

        public string PastResultsMarkdown { get; set; } = string.Empty;

        public string CourseMapMarkdown { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<DashFile>? Files { get; set; }
    }
}
