using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PorteHobe.API.DTOs;
using PorteHobe.API.Services;

namespace PorteHobe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BadgesController : ControllerBase
    {
        private readonly IBadgeService _badgeService;

        public BadgesController(IBadgeService badgeService)
        {
            _badgeService = badgeService;
        }

        /// <summary>
        /// Get all badges for the authenticated user
        /// </summary>
        /// <param name="fromDate">Optional: Start date for badge calculation (default: 1 year ago)</param>
        /// <param name="toDate">Optional: End date for badge calculation (default: today)</param>
        /// <returns>List of all badges with achievement status</returns>
        [HttpGet]
        public async Task<ActionResult<UserBadgesResponseDto>> GetUserBadges(
            [FromQuery] DateOnly? fromDate = null,
            [FromQuery] DateOnly? toDate = null)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var badges = await _badgeService.GetUserBadgesAsync(userId, fromDate, toDate);
            return Ok(badges);
        }

        /// <summary>
        /// Get a specific badge status
        /// </summary>
        /// <param name="badgeType">The type of badge to retrieve</param>
        /// <returns>Badge details with achievement status</returns>
        [HttpGet("{badgeType}")]
        public async Task<ActionResult<BadgeDto>> GetBadge(BadgeType badgeType)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var badge = await _badgeService.GetBadgeAsync(userId, badgeType);
            
            if (badge == null)
                return NotFound($"Badge of type {badgeType} not found");

            return Ok(badge);
        }

        /// <summary>
        /// Check if user has earned any new badges since a specific date
        /// </summary>
        /// <param name="sinceDate">Date to check from</param>
        /// <returns>Boolean indicating if new badges were earned</returns>
        [HttpGet("new-badges/{sinceDate}")]
        public async Task<ActionResult<dynamic>> HasNewBadges(DateOnly sinceDate)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User ID not found in token");

            var hasNew = await _badgeService.HasNewBadgesAsync(userId, sinceDate);
            
            return Ok(new { hasNewBadges = hasNew, sincDate = sinceDate });
        }
    }
}
