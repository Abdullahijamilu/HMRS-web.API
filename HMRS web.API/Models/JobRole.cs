using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HMRS_web.API.Models;

public partial class JobRole
{
    [Key]
    public Guid Id { get; set; } 

    [Required]
    [StringLength(100)]
    public string Title { get; set; } = null!;

    [StringLength(255)]
    public string? Description { get; set; } 

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>(); 
}