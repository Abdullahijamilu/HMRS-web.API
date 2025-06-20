using System.ComponentModel.DataAnnotations;
using HMRS_web.API.Models;

namespace HMRS_web.API.DTO
{
    public class EReadDTO
    {
        [Required]
        public string FullName { get; set; } = null!;
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Address { get; set; }
        public Guid? DepartmentId { get; set; }
        public DateTime HireDate { get; set; }
        public string DepartmentName { get; set; }
        public string? UserName { get; set; }
        public string? Role { get; set; }
        public virtual JobRole? JobRole { get; set; }
    }
}
