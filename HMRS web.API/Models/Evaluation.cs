using System;
using System.Collections.Generic;

namespace HMRS_web.API.Models;

public partial class Evaluation
{
    public Guid Id { get; set; }

    public Guid EmployeeId { get; set; }

    public Guid ReviewerId { get; set; }

    public int Score { get; set; }

    public string? Remarks { get; set; }

    public DateOnly EvaluationDate { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Employee Reviewer { get; set; } = null!;
}
