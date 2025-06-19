using Microsoft.AspNetCore.Identity;

namespace HMRS_web.API.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
