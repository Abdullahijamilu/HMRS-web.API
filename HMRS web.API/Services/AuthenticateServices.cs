using HMRS_web.API.DTO;
using HMRS_web.API.Models;
using HMRS_web.API.Services.Interface;
using Microsoft.AspNetCore.Identity;

namespace HMRS_web.API.Services
{
    public class AuthenticateServices : IAuthenticateServices
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly HmrsContext _Context;
        private readonly JwtServices _jwtServices;
        private readonly ILogger<AuthenticateServices> _logger;
        private readonly RoleManager<IdentityRole> _roleManager;
            
        public AuthenticateServices(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, 
            HmrsContext hmrsContext, JwtServices jwtServices, ILogger<AuthenticateServices> logger, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _Context = hmrsContext;
            _jwtServices = jwtServices;
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task<IdentityResult> RegisterUser(RegisterDTO user)
        {
            var newUser = new ApplicationUser
            {
                UserName = user.UserName,
                Email = user.Email
                // Note: Role is not a property of ApplicationUser; it's assigned via AddToRoleAsync
            };

            var result = await _userManager.CreateAsync(newUser, user.Password);
            if (result.Succeeded)
            {
                // Validate role
                var validRoles = new[] { "Admin", "Manager", "Employee" };
                if (!validRoles.Contains(user.Role))
                {
                    return IdentityResult.Failed(new IdentityError { Description = $"Invalid role: {user.Role}. Must be Admin, Manager, or Employee." });
                }

                // Ensure role exists
                if (!await _roleManager.RoleExistsAsync(user.Role))
                {
                    var roleResult = await _roleManager.CreateAsync(new IdentityRole(user.Role));
                    if (!roleResult.Succeeded)
                    {
                        return IdentityResult.Failed(roleResult.Errors.ToArray());
                    }
                }

                // Assign the role to the user
                var addRoleResult = await _userManager.AddToRoleAsync(newUser, user.Role);
                if (!addRoleResult.Succeeded)
                {
                    return addRoleResult;
                }
            }

            return result;
        }

        public async Task<ResponseModel<LoginDTO>> ValidateUser(LoginDTO model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return new ResponseModel<LoginDTO>
                {
                    IsSuccess = false,
                    Message = "User not found"
                };

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
                return new ResponseModel<LoginDTO>
                {
                    IsSuccess = false,
                    Message = "Wrong Password"
                };

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtServices.GenerateToken(user, roles.FirstOrDefault() ?? "User");

            return new ResponseModel<LoginDTO>
            {
                IsSuccess = true,
                Message = "Login successful",
                Data = new LoginDTO
                {
                    Token = token,
                    UserId = user.Id,
                    Email = user.Email,
                    Role = roles.FirstOrDefault() ?? "User"
                }
            };
        }
    }
}
