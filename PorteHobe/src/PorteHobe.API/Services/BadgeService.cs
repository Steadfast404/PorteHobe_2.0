using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PorteHobe.API.DTOs;
using Portehobe.src.PorteHobe.Infrastructure;

namespace PorteHobe.API.Services
{
    public class BadgeService : IBadgeService
    {
        private readonly AppDbContext _context;
        private readonly IStudySessionService _studySessionService;

        public BadgeService(AppDbContext context, IStudySessionService studySessionService)
        {
            _context = context;
            _studySessionService = studySessionService;
        }

        public async Task<UserBadgesResponseDto> GetUserBadgesAsync(
            string userId,
            DateOnly? fromDate = null,
            DateOnly? toDate = null)
        {
            // Default to last 365 days if not specified
            toDate ??= DateOnly.FromDateTime(DateTime.UtcNow);
            fromDate ??= toDate.Value.AddDays(-365);

            // Get streak summary which includes daily summaries
            var streakSummary = await _studySessionService.GetStreakSummaryAsync(
                userId,
                fromDate.Value,
                toDate.Value,
                minDailySeconds: 0); // Get all days, even partial study days

            // Get total study sessions count
            var sessionCount = await _context.StudySessions
                .CountAsync(s => s.AppUserId == userId && s.EndTime != null);

            // Calculate total hours
            var totalSessions = await _context.StudySessions
                .Where(s => s.AppUserId == userId && s.EndTime != null)
                .ToListAsync();

            var totalHours = totalSessions
                .Sum(s => (s.EndTime!.Value - s.StartTime).TotalSeconds) / 3600.0;

            var badges = new List<BadgeDto>();

            // Daily Streak Badge
            badges.Add(CreateDailyStreakBadge(streakSummary));

            // 7-Day Streak Badge
            badges.Add(CreateStreakBadge(streakSummary.CurrentStreakDays, 7, "SevenDayStreak",
                "Week Warrior", "Studied 7 consecutive days"));

            // 30-Day Streak Badge
            badges.Add(CreateStreakBadge(streakSummary.CurrentStreakDays, 30, "ThirtyDayStreak",
                "Monthly Master", "Studied 30 consecutive days"));

            // 90-Day Streak Badge
            badges.Add(CreateStreakBadge(streakSummary.CurrentStreakDays, 90, "NinetyDayStreak",
                "Dedicated Scholar", "Studied 90 consecutive days"));

            // 50 Hours Badge
            badges.Add(CreateHoursMilestone(totalHours, 50, "FiftyHoursStudy",
                "50-Hour Achiever", "Completed 50 hours of study"));

            // 100 Hours Badge
            badges.Add(CreateHoursMilestone(totalHours, 100, "HundredHoursStudy",
                "Centurion Scholar", "Completed 100 hours of study"));

            // 500 Hours Badge
            badges.Add(CreateHoursMilestone(totalHours, 500, "FiveHundredHoursStudy",
                "Legendary Learner", "Completed 500 hours of study"));

            // 50 Sessions Badge
            badges.Add(CreateSessionsMilestone(sessionCount, 50, "FiftySessionsMilestone",
                "Session Master", "Completed 50 study sessions"));

            return new UserBadgesResponseDto
            {
                Badges = badges,
                TotalBadgesEarned = badges.Count(b => b.Achieved)
            };
        }

        public async Task<BadgeDto?> GetBadgeAsync(string userId, BadgeType badgeType)
        {
            var allBadges = await GetUserBadgesAsync(userId);
            return allBadges.Badges.FirstOrDefault(b => b.Type == badgeType);
        }

        public async Task<bool> HasNewBadgesAsync(string userId, DateOnly sinceDate)
        {
            var badges = await GetUserBadgesAsync(userId);
            return badges.Badges.Any(b => b.Achieved && b.AchievedDate >= sinceDate);
        }

        private BadgeDto CreateDailyStreakBadge(StudyStreakSummaryDto streakSummary)
        {
            var todayStudied = streakSummary.TodayTotalSeconds > 0;

            return new BadgeDto
            {
                Type = BadgeType.DailyStreak,
                Title = "Daily Scholar",
                Description = "Studied at least some time today",
                IconName = "daily-scholar",
                Achieved = todayStudied,
                AchievedDate = todayStudied ? DateOnly.FromDateTime(DateTime.UtcNow) : null
            };
        }

        private BadgeDto CreateStreakBadge(
            int currentStreak,
            int targetStreak,
            string iconName,
            string title,
            string description)
        {
            var achieved = currentStreak >= targetStreak;

            return new BadgeDto
            {
                Type = (BadgeType)Enum.Parse(typeof(BadgeType), targetStreak switch
                {
                    7 => "SevenDayStreak",
                    30 => "ThirtyDayStreak",
                    90 => "NinetyDayStreak",
                    _ => "DailyStreak"
                }),
                Title = title,
                Description = description,
                IconName = iconName,
                Achieved = achieved,
                Progress = achieved ? targetStreak : currentStreak,
                ProgressTarget = targetStreak,
                AchievedDate = achieved ? DateOnly.FromDateTime(DateTime.UtcNow) : null
            };
        }

        private BadgeDto CreateHoursMilestone(
            double totalHours,
            int targetHours,
            string iconName,
            string title,
            string description)
        {
            var achieved = totalHours >= targetHours;

            return new BadgeDto
            {
                Type = (BadgeType)Enum.Parse(typeof(BadgeType), targetHours switch
                {
                    50 => "FiftyHoursStudy",
                    100 => "HundredHoursStudy",
                    500 => "FiveHundredHoursStudy",
                    _ => "FiftyHoursStudy"
                }),
                Title = title,
                Description = description,
                IconName = iconName,
                Achieved = achieved,
                Progress = (int)Math.Floor(totalHours),
                ProgressTarget = targetHours,
                AchievedDate = achieved ? DateOnly.FromDateTime(DateTime.UtcNow) : null
            };
        }

        private BadgeDto CreateSessionsMilestone(
            int sessionCount,
            int targetSessions,
            string iconName,
            string title,
            string description)
        {
            var achieved = sessionCount >= targetSessions;

            return new BadgeDto
            {
                Type = (BadgeType)Enum.Parse(typeof(BadgeType), "FiftySessionsMilestone"),
                Title = title,
                Description = description,
                IconName = iconName,
                Achieved = achieved,
                Progress = sessionCount,
                ProgressTarget = targetSessions,
                AchievedDate = achieved ? DateOnly.FromDateTime(DateTime.UtcNow) : null
            };
        }
    }
}
