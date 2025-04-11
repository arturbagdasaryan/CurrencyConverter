using CurrencyConverter.Api.Models;
using CurrencyConverter.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CurrencyConverter.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // TODO: Replace this with real user validation
            if (request.Email == "admin@currencyapi.com" && request.Password == "P@ssw0rd")
            {
                var roles = new List<string> { "Admin" };
                var token = _jwtTokenService.GenerateToken("1", request.Email, roles);
                return Ok(new { Token = token });
            }

            return Unauthorized();
        }
    }
}
