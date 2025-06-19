using HMRS_web.API.DTO;
using HMRS_web.API.Models;
using Microsoft.AspNetCore.Identity;

namespace HMRS_web.API.Services.Interface
{
    public interface IAuthenticateServices
    {
        Task<IdentityResult> RegisterUser(RegisterDTO user);
        Task<ResponseModel<LoginDTO>> ValidateUser(LoginDTO model);
    }
}
