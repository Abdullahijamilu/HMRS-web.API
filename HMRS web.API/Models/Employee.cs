using System;
using System.Collections.Generic;

namespace HMRS_web.API.Models;

public partial class Employee
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Phone { get; set; }

    public string? Address { get; set; }

    public Guid? DepartmentId { get; set; }

    public Guid? JobRoleId { get; set; }

    public DateOnly? HireDate { get; set; }

    public string? CvfilePath { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<Evaluation> EvaluationEmployees { get; set; } = new List<Evaluation>();

    public virtual ICollection<Evaluation> EvaluationReviewers { get; set; } = new List<Evaluation>();

    public virtual JobRole? JobRole { get; set; }

    public virtual ICollection<LeaveRequest> LeaveRequestApprovedByNavigations { get; set; } = new List<LeaveRequest>();

    public virtual ICollection<LeaveRequest> LeaveRequestEmployees { get; set; } = new List<LeaveRequest>();

    public virtual User User { get; set; } = null!;
}
