using System;

namespace HMRS_web.API.DTO
{
    public class JobRoleReadDTO
    {
        public Guid Id { get; set; } // Job role ID

        public string Title { get; set; } = null!; // e.g., "Software Developer"

        public string? Description { get; set; } // Optional description

        public int EmployeeCount { get; set; } // Number of employees with this role
    }
}