using HMRS_web.API.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace HMRS_web.API.DTO
{
    public class LeaveRequestReadDTO
    {
        public Guid Id { get; set; }

        public Guid EmployeeId { get; set; }

        public DateOnly FromDate { get; set; }

        public DateOnly ToDate { get; set; }

        public string? Reason { get; set; }

        public string? Status { get; set; }

        public DateTime? RequestedAt { get; set; }

        public Guid? ApprovedBy { get; set; }

        public virtual Employee? ApprovedByNavigation { get; set; }
        [ForeignKey("EmployeeId")]
        public virtual Employee Employee { get; set; } = null!;

    }
}
