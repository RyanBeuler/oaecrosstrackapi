using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.DTOs.Athletes
{
    public class CreateAthleteDto
    {
        [Required]
        [StringLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public required string LastName { get; set; }

        [Required]
        [Range(2000, 2100)]
        public int GraduationYear { get; set; }

        [Required]
        [StringLength(1)]
        [RegularExpression("^[MF]$", ErrorMessage = "Gender must be 'M' or 'F'")]
        public required string Gender { get; set; }
    }

    public class UpdateAthleteDto
    {
        [Required]
        [StringLength(50)]
        public required string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public required string LastName { get; set; }

        [Required]
        [Range(2000, 2100)]
        public int GraduationYear { get; set; }

        [Required]
        [StringLength(1)]
        [RegularExpression("^[MF]$", ErrorMessage = "Gender must be 'M' or 'F'")]
        public required string Gender { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class AthleteResponseDto
    {
        public int Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public int GraduationYear { get; set; }
        public required string Gender { get; set; }
        public bool IsActive { get; set; }
        public required string GradeLevel { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
