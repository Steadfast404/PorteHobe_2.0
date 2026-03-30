using System.Threading.Tasks;
using PorteHobe.API.DTOs;

namespace PorteHobe.API.Services
{
    public interface IBadgeService
    {
        /// <summary>
        /// Get all badges for a user based on their study streaks and session history
        /// </summary>
        Task<UserBadgesResponseDto> GetUserBadgesAsync(string userId, DateOnly? fromDate = null, DateOnly? toDate = null);

        /// <summary>
        /// Get specific badge achievement status
        /// </summary>
        Task<BadgeDto?> GetBadgeAsync(string userId, BadgeType badgeType);

        /// <summary>
        /// Check if user achieved a new badge (useful for notifications)
        /// </summary>
        Task<bool> HasNewBadgesAsync(string userId, DateOnly sinceDate);
    }
}
