using System;
using System.Collections.Generic;

namespace HMRS_web.API.Models;

public partial class JobRole
{
    public Guid Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
