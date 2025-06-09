using HMRS_web.API.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HMRS_web.API.Services
{
    public class JwtServices
    {
        private readonly IConfiguration _config;
        public JwtServices(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateToken(User user, string role)
        {
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:key"]));
            var credentials = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var claims = new Claim[]
            {
                new Claim (ClaimTypes.Surname, user.UserName),
                new Claim (ClaimTypes.Role, role),
                new Claim (ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            JwtSecurityToken jwtSecurityToken = new(
                issuer: _config["jwt:issuer"],
                audience: _config["jwt:audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_config["jwt:ExpireMinutes"])),
                signingCredentials: credentials);


            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}


