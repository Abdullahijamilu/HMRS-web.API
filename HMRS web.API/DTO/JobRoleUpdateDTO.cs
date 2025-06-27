using System;
using System.ComponentModel.DataAnnotations;

namespace HMRS_web.API.DTO
{
    public class JobRoleUpdateDTO
    {
        public Guid Id { get; set; } // Job role ID

        [StringLength(100)]
        public string? Title { get; set; } // Optional title update

        [StringLength(255)]
        public string? Description { get; set; } // Optional description update
    }
}