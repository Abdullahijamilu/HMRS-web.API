using System;
using System.Collections.Generic;

namespace HMRS_web.API.Models;

public partial class File
{
    public Guid Id { get; set; }

    public string? FileName { get; set; }

    public string? FileType { get; set; }

    public string? FilePath { get; set; }

    public Guid? UploadedBy { get; set; }

    public DateTime? UploadedAt { get; set; }

    public virtual User? UploadedByNavigation { get; set; }
}
