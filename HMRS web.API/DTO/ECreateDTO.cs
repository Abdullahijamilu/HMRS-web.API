using System;
using System.ComponentModel.DataAnnotations;

namespace HMRS_web.API.DTO
{
    public class ECreateDTO
    {
        public Guid Id { get; set; } // Employee ID (auto-generated)

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!; // e.g., "Sarah Jones"

        [Phone]
        public string? Phone { get; set; } // e.g., "555-1234"

        [StringLength(200)]
        public string? Address { get; set; } // e.g., "123 Main St"

        [Required]
        public Guid DepartmentId { get; set; } // Department ID

        [Required]
        public Guid JobRoleId { get; set; } // Job role ID

        [Required]
        public DateOnly? HireDate { get; set; } // e.g., "2024-07-01"

        public string? UserName { get; set; } // Optional username for account

        [EmailAddress]
        public string? Email { get; set; } // Optional email for account

        public string? Password { get; set; } // Optional password for account

        public string? Role { get; set; } // Optional role, e.g., "Employee"
    }
}