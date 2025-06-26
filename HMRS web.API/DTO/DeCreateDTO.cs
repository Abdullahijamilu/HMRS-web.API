using System.ComponentModel.DataAnnotations;

namespace HMRS_web.API.DTO
{
    public class DeCreateDTO
    {
        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}
