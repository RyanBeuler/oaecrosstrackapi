using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.DTOs.Sports
{
    public class CreateSportDto
    {
        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        [StringLength(20)]
        public required string Season { get; set; }

        public int DisplayOrder { get; set; }
    }

    public class UpdateSportDto
    {
        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        [StringLength(20)]
        public required string Season { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class SportResponseDto
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Season { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
