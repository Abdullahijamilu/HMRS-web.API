using HMRS_web.API.DTO;
using HMRS_web.API.Models;
using HMRS_web.API.Services;
using HMRS_web.API.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HMRS_web.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly HmrsContext _context;
        private readonly IAuthenticateServices _authenticateServices;

        public UserController(HmrsContext hmrsContext, IAuthenticateServices authenticateServices)
        {
            _context = hmrsContext;
            _authenticateServices = authenticateServices;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _authenticateServices.ValidateUser(model);

            if (!result.IsSuccess)
            {
                return Unauthorized(new { Message = result.Message });
            }

            return Ok(new
            {
                Message = "Login successful",
                Token = result.Data
            });
        }
        [HttpPost("Register")]
        public async Task<IActionResult> CreateUser([FromBody] RegisterDTO request)
        {
            var response = await _authenticateServices.RegisterUser(request);
            return Ok(response);
        }
    }
}

