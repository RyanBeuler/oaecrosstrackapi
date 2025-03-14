using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using OaeCrosstrackApi.DTOs;
using OaeCrosstrackApi.Services;

namespace OaeCrosstrackApi.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequest)
        {
            var response = await _authService.LoginAsync(loginRequest);
            
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }
            
            return Ok(response);
        }

        
    }
}