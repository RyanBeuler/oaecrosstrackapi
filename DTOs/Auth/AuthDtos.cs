using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.DTOs
{
    public class LoginRequestDto
    {
        [Required]
        public required string Username { get; set; }
        
        [Required]
        public required string Password { get; set; }
    }

    public class LoginResponseDto
    {
        public required string Token { get; set; }
        public required string Username { get; set; }
        public int UserId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
    }
}