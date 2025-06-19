namespace HMRS_web.API.DTO
{
    public class LoginDTO
    {
        public string Token { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }
}

