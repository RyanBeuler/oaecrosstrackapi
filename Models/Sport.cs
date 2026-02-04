using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.Models
{
    public class Sport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? Name { get; set; }

        [Required]
        [StringLength(20)]
        public string? Season { get; set; } // Fall, Winter, Spring, Special

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation property
        public ICollection<Event>? Events { get; set; }
    }
}
