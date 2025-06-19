using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace HMRS_web.API.DTO
{
    public class RegisterDTO
    {
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        [Required]
        [RegularExpression("Admin|Manager|Employee", ErrorMessage = "Role must be Admin, Manager, or Employee")]
        public string Role { get; set; }
    }
}
