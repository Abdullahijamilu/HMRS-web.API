using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HMRS_web.API.Models;

public partial class JobRole
{
    [Key]
    public Guid Id { get; set; } // Primary key (GUID)

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = null!; // e.g., "Software Developer"

    [StringLength(255)]
    public string? Description { get; set; } // e.g., "Develops software applications"

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>(); // Employees with this role
}