using System.ComponentModel.DataAnnotations;
using HMRS_web.API.Models;

namespace HMRS_web.API.DTO
{
    public class ECreateDTO
    {
      
        [Required]
        public string FullName { get; set; } = null!;
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Address { get; set; }
        public Guid? DepartmentId { get; set; }
        [Required]
        public DateOnly? HireDate { get; set; }
        public string? UserName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Role { get; set; }
        public object Id { get; internal set; }
    }
}
