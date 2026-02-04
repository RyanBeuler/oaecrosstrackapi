using System;
using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.Models
{
    public class Athlete
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Required]
        public int GraduationYear { get; set; }

        [Required]
        [StringLength(1)]
        public string? Gender { get; set; } // "M" or "F"

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
