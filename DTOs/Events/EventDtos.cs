using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.DTOs.Events
{
    public class CreateEventDto
    {
        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        public int SportId { get; set; }

        [Required]
        [StringLength(20)]
        public required string EventType { get; set; } // Running, Field, Relay

        public int SortOrder { get; set; }
    }

    public class UpdateEventDto
    {
        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        public int SportId { get; set; }

        [Required]
        [StringLength(20)]
        public required string EventType { get; set; }

        public int SortOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class EventResponseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int SportId { get; set; }
        public required string SportName { get; set; }
        public required string EventType { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
