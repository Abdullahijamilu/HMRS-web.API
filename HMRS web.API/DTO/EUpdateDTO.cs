using System.ComponentModel.DataAnnotations;

namespace HMRS_web.API.DTO
{
    public class EUpdateDTO
    {
        public Guid Id { get; set; }
        [Required]
        public string FullName { get; set; } = null!;
        [Required]
        public string? Phone { get; set; }
        [Required]
        public string? Address { get; set; }
        public Guid? DepartmentId { get; set; }
    }
}
