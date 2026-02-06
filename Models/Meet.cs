using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OaeCrosstrackApi.Models
{
    public class Meet
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int SportId { get; set; }

        [ForeignKey("SportId")]
        public Sport? Sport { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [Required]
        public DateTime MeetDate { get; set; }

        [Required]
        [StringLength(20)]
        public required string MeetType { get; set; } // Practice, Dual, Invitational, Championship

        [StringLength(100)]
        public string? Opponent { get; set; }

        public bool IsHome { get; set; } = true;

        public int? OurScore { get; set; }

        public int? OpponentScore { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
