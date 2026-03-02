using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Portehobe.API.Models;
using Portehobe.Infrastructure.Services;
using Portehobe.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Portehobe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ITokenBlacklistService _blacklistService;

        public AuthController(UserManager<AppUser> userManager, IConfiguration configuration, ITokenBlacklistService blacklistService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _blacklistService = blacklistService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var email = model.Email.ToLower().Trim();

            var userExists = await _userManager.FindByEmailAsync(email);
            if (userExists != null)
                return BadRequest("Email already registered");

            var user = new AppUser
            {
                UserName = email,
                Email = email,
                FullName = model.FullName.Trim(),


            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var email = model.Email.ToLower().Trim();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
                return Unauthorized("Invalid credentials");

            var roles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            foreach (var role in roles)
                authClaims.Add(new Claim(ClaimTypes.Role, role));

            var jwtKey = _configuration["Jwt:Key"] ?? "DefaultSuperSecretKeyForDevelopmentOnly123!";
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            var authHeader = Request.Headers["Authorization"].ToString();
            string? token = null;

            if (System.Net.Http.Headers.AuthenticationHeaderValue.TryParse(authHeader, out var headerValue) &&
                string.Equals(headerValue.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
            {
                token = headerValue.Parameter;
            }

            if (!string.IsNullOrEmpty(token))
            {
                _blacklistService.BlacklistToken(token);
            }
            return Ok("Logged out successfully");
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email.ToLower().Trim());
            if (user == null)
            {
                // For security reasons, don't reveal that the user does not exist
                return Ok("If the email is registered, a password reset link has been sent.");
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            // In a real app, you would email this token to the user
            // For now, we'll return it in the response for demonstration
            return Ok(new { token, message = "Password reset token generated." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email.ToLower().Trim());
            if (user == null)
                return BadRequest("Invalid request");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.NewPassword);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok("Password has been reset successfully.");
        }
    }
}
