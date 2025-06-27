using System;

namespace HMRS_web.API.DTO
{
    public class EReadDTO
    {
        public Guid Id { get; set; } // Employee ID

        public string FullName { get; set; } = null!; // e.g., "Sarah Jones"

        public string? Phone { get; set; } // e.g., "555-1234"

        public string? Address { get; set; } // e.g., "123 Main St"

        public Guid? DepartmentId { get; set; } // Department ID

        public string? DepartmentName { get; set; } // e.g., "IT"

        public Guid? JobRoleId { get; set; } // Job role ID

        public string? JobRoleTitle { get; set; } // e.g., "Software Developer"

        public DateTime HireDate { get; set; } // e.g., "2024-07-01"

        public string? UserName { get; set; } // e.g., "sarah.jones"

        public string? Role { get; set; } // e.g., "Employee"
    }
}