using System;
using System.Collections.Generic;

namespace HMRS_web.API.Models;

public partial class LeaveRequest
{
    public int Id { get; set; }

    public Guid EmployeeId { get; set; }

    public DateOnly FromDate { get; set; }

    public DateOnly ToDate { get; set; }

    public string? Reason { get; set; }

    public string? Status { get; set; }

    public DateTime? RequestedAt { get; set; }

    public Guid? ApprovedBy { get; set; }

    public virtual Employee? ApprovedByNavigation { get; set; }

    public virtual Employee Employee { get; set; } = null!;
}
