using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace HMRS_web.API.Models;

public partial class User : IdentityUser<Guid>
{
    public Guid Id { get; set; }

    public Guid JobRoleId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<File> Files { get; set; } = new List<File>();

    public virtual JobRole JobRole { get; set; } = null!;
}

public class ApplicationRole : IdentityRole<Guid>
{
    // Optional: Add any custom properties for your roles
}