using System;
using System.ComponentModel.DataAnnotations;

namespace HMRS_web.API.DTO
{
    public class RegisterDTO
    {
        [Required]
        public string UserName { get; set; } = null!; // e.g., "sarah.jones"

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!; // e.g., "sarah@example.com"

        [Required]
        public string Password { get; set; } = null!; // e.g., "Password123!"

        public string? Role { get; set; } // e.g., "Employee"

        public Guid Id { get; set; } // User ID (auto-generated)
    }
}