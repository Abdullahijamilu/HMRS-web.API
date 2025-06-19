using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMRS_web.API.Models;

public partial class File
{
    public Guid Id { get; set; }

    public string? FileName { get; set; }

    public string? FileType { get; set; }

    public string? FilePath { get; set; }

    public string? UploadedBy { get; set; }

    public DateTime? UploadedAt { get; set; }
    [ForeignKey("UploadedBy")]
    public virtual ApplicationUser User { get; set; }
}
