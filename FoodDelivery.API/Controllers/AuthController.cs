using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using FoodDelivery.API.Models;
using FoodDelivery.API.Models.DTOs;
using FoodDelivery.API.Services;
using Microsoft.IdentityModel.JsonWebTokens;

namespace FoodDelivery.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JwtTokenService _jwtService;

        public AuthController(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            JwtTokenService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, dto.Role);
            return Ok("Registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password))
                return Unauthorized("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtService.CreateToken(user, roles);

            // Store JWT in HttpOnly cookie
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddHours(3)
            });

            return Ok(new { Token = token });
        }

        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var email = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
            if (email == null) return Unauthorized();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return Unauthorized();

            var roles = await _userManager.GetRolesAsync(user);
            return Ok(new { user.Email, user.FullName, Roles = roles });
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    IsGoogleUser = true
                };

                var result = await _userManager.CreateAsync(user);
                if (!result.Succeeded)
                    return BadRequest(result.Errors);

                await _userManager.AddToRoleAsync(user, "User");
            }

            var roles = await _userManager.GetRolesAsync(user);

            // Generate JWT token
            var token = _jwtService.CreateToken(user, roles);
            return Ok(token);
        }

    }
}
