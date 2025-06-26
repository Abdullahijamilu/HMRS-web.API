using System.ComponentModel.DataAnnotations;

namespace HMRS_web.API.DTO
{
    public class JobRoleCreateDTO
    {
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string? Description { get; set; }
    }
}
