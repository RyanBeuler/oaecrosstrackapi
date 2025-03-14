using System;
using System.ComponentModel.DataAnnotations;

namespace OaeCrosstrackApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(50)]
        public string? Username { get; set; }
        
        [Required]
        public string? PasswordHash { get; set; }
        
        public string? Email { get; set; }
        
        public string? FirstName { get; set; }
        
        public string? LastName { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime? LastLoginAt { get; set; }
    }
}