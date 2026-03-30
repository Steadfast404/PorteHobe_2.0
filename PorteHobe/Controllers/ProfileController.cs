using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portehobe.Model;
using PorteHobe.API.DTOs;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PorteHobe.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;

        public ProfileController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        // ১. Profile fetch kora (Figma-r header r fields fill-up korar jonno)
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            return Ok(new
            {
                user.FullName,
                user.Email,
                user.Bio,
                user.Timezone,
                user.Language,
                user.Theme,
                user.DateFormat,
                user.StudyReminders,
                user.GoalAchievements,
                user.WeeklyReports,
                user.PushNotifications,
                user.StudyStreak,
                user.CreatedAt
            });
        }

        // ২. Profile update kora (Figma-r 'Save Changes' button click korle)
        [HttpPut("update")]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateDto dto)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Mapping DTO to User Model
            user.FullName = dto.FullName;
            user.Bio = dto.Bio;
            user.Timezone = dto.Timezone;
            user.Language = dto.Language;
            user.Theme = dto.Theme;
            user.DateFormat = dto.DateFormat;
            user.StudyReminders = dto.StudyReminders;
            user.GoalAchievements = dto.GoalAchievements;
            user.WeeklyReports = dto.WeeklyReports;
            user.PushNotifications = dto.PushNotifications;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                return Ok(new { message = "Profile updated successfully!" });
            }

            return BadRequest(result.Errors);
        }
    }
}