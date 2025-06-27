using System;
using System.ComponentModel.DataAnnotations;

namespace HMRS_web.API.DTO
{
    public class EUpdateDTO
    {
        public Guid Id { get; set; } // Employee ID

        [StringLength(100)]
        public string? FullName { get; set; } 

        [StringLength(20)]
        [Phone]
        public string? Phone { get; set; } // e.g., "555-1234"

        [StringLength(200)]
        public string? Address { get; set; } // e.g., "123 Main St"

        public Guid? DepartmentId { get; set; } // Department ID

        public Guid? JobRoleId { get; set; } // Job role ID

        public DateOnly? HireDate { get; set; } // e.g., "2024-07-01"
    }
}