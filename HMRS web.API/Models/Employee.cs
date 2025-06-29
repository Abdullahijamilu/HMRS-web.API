using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMRS_web.API.Models
{
    public partial class Employee
    {
        [Key]
        public Guid Id { get; set; } // Primary key (GUID)

        public string? UserId { get; set; } // Nullable to allow employees without accounts

        [Required]
        [StringLength(100)]
        public string FullName { get; set; } = null!; // e.g., "Sarah Jones"

        [StringLength(20)]
        [Phone]
        public string? Phone { get; set; } // e.g., "555-1234"

        [StringLength(200)]
        public string? Address { get; set; } // e.g., "123 Main St"

        public Guid? DepartmentId { get; set; } // Foreign key to Department

        public Guid? JobRoleId { get; set; } // Foreign key to JobRole

        public DateOnly? HireDate { get; set; } 

        [StringLength(200)]
        public string? CvfilePath { get; set; } 

        [ForeignKey("DepartmentId")]
        public virtual Department? Department { get; set; } 

        [ForeignKey("JobRoleId")]
        public virtual JobRole? JobRole { get; set; } 

        [ForeignKey("UserId")]
        public virtual ApplicationUser? User { get; set; } 

        public virtual ICollection<Evaluation> EvaluationEmployees { get; set; } = new List<Evaluation>();

        public virtual ICollection<Evaluation> EvaluationReviewers { get; set; } = new List<Evaluation>();

        public virtual ICollection<LeaveRequest> LeaveRequestApprovedByNavigations { get; set; } = new List<LeaveRequest>();

        public virtual ICollection<LeaveRequest> LeaveRequestEmployees { get; set; } = new List<LeaveRequest>();

        public static implicit operator Employee?(string? v)
        {
            throw new NotImplementedException();
        }
    }
}