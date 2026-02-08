using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OaeCrosstrackApi.Models
{
    public class RosterEntry
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AthleteId { get; set; }

        [ForeignKey("AthleteId")]
        public Athlete? Athlete { get; set; }

        [Required]
        public int SportId { get; set; }

        [ForeignKey("SportId")]
        public Sport? Sport { get; set; }

        [Required]
        public int Year { get; set; } // School year (e.g., 2025 for 2024-2025 school year)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
