using System;

namespace HMRS_web.API.DTO
{
    public class DeReadDTO
    {
        public Guid Id { get; set; } // Department ID

        public string Name { get; set; } = null!; // e.g., "IT"

        public string? Description { get; set; } // Optional description

        public int EmployeeCount { get; set; } // Number of employees in department
    }
}