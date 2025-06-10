using HMRS_web.API.DTO;
using HMRS_web.API.Models;
using HMRS_web.API.Services.Interface;
using Microsoft.AspNetCore.Identity;

namespace HMRS_web.API.Services
{
    public class AuthenticateServices : IAuthenticateServices
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly HmrsContext _Context;
        private readonly JwtServices _jwtServices;

        public AuthenticateServices(UserManager<User> userManager, SignInManager<User> signInManager, HmrsContext hmrsContext, JwtServices jwtServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _Context = hmrsContext;
            _jwtServices = jwtServices;
        }

        public async Task<IdentityResult> RegisterUser(RegisterDTO user)
        {
            var newUser = new User
            {
                UserName = user.UserName,
                Email = user.Email,
                //Role = user.Role
            };

            var result = await _userManager.CreateAsync(newUser, user.Password);

            if (result.Succeeded )
            {
                await _userManager.AddToRoleAsync(newUser, user.UserName);
            }

            return result;
        }

        public async Task<ResponseModel<User>> ValidateUser(LoginDTO model)
        {
            if (model == null)
            {
                return new ResponseModel<User>
                {
                    IsSuccess = false,
                    Message = "Invalid login request"
                };
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return new ResponseModel<User>
                {
                    IsSuccess = false,
                    Message = "User not found"
                };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded)
            {
                return new ResponseModel<User>
                {
                    IsSuccess = false,
                    Message = "Wrong password"
                };
            }

            return new ResponseModel<User>
            {
                IsSuccess = true,
                Message = "Login successful",
            };
        }

    }
}
