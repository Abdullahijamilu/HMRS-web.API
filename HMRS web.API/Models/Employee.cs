using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMRS_web.API.Models;

public partial class Employee
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? JobRoleId { get; set; }

    public DateOnly? HireDate { get; set; }

    public string? CvfilePath { get; set; }
    [ForeignKey("DepartmentId")]
    public virtual Department? Department { get; set; }
    [ForeignKey("UploadedBy")]
    public virtual ApplicationUser User { get; set; }

    public virtual ICollection<Evaluation> EvaluationEmployees { get; set; } = new List<Evaluation>();

    public virtual ICollection<Evaluation> EvaluationReviewers { get; set; } = new List<Evaluation>();

    public virtual JobRole? JobRole { get; set; }

    public virtual ICollection<LeaveRequest> LeaveRequestApprovedByNavigations { get; set; } = new List<LeaveRequest>();

    public virtual ICollection<LeaveRequest> LeaveRequestEmployees { get; set; } = new List<LeaveRequest>();
}
