using System;

namespace PorteHobe.API.DTOs
{
    public enum BadgeType
    {
        DailyStreak,           // Studied today
        SevenDayStreak,        // 7 consecutive study days
        ThirtyDayStreak,       // 30 consecutive study days
        NinetyDayStreak,       // 90 consecutive study days
        FiftyHoursStudy,       // 50 total hours studied
        HundredHoursStudy,     // 100 total hours studied
        FiveHundredHoursStudy, // 500 total hours studied
        FiftySessionsMilestone // 50 completed study sessions
    }

    public class BadgeDto
    {
        public BadgeType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconName { get; set; } = string.Empty; // For frontend icon mapping
        public bool Achieved { get; set; }
        public DateOnly? AchievedDate { get; set; }
        public int? Progress { get; set; } // For badges with milestones (e.g., days, hours)
        public int? ProgressTarget { get; set; }
    }

    public class UserBadgesResponseDto
    {
        public List<BadgeDto> Badges { get; set; } = new();
        public int TotalBadgesEarned { get; set; }
    }
}
