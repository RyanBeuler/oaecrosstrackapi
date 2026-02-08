using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.DTOs.Rosters
{
    public class CreateRosterEntryDto
    {
        [Required]
        public int AthleteId { get; set; }

        [Required]
        public int SportId { get; set; }

        [Required]
        [Range(2000, 2100)]
        public int Year { get; set; }
    }

    public class BulkAddRosterEntriesDto
    {
        [Required]
        public required int[] AthleteIds { get; set; }

        [Required]
        public int SportId { get; set; }

        [Required]
        [Range(2000, 2100)]
        public int Year { get; set; }
    }

    public class RosterEntryResponseDto
    {
        public int Id { get; set; }
        public int AthleteId { get; set; }
        public required string AthleteFirstName { get; set; }
        public required string AthleteLastName { get; set; }
        public required string AthleteGradeLevel { get; set; }
        public required string AthleteGender { get; set; }
        public int SportId { get; set; }
        public required string SportName { get; set; }
        public int Year { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RosterQueryDto
    {
        public int? SportId { get; set; }
        public int? Year { get; set; }
    }
}
