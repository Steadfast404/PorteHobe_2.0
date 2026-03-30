using System.ComponentModel.DataAnnotations;

namespace PorteHobe.API.DTOs
{
    public class ProfileUpdateDto
    {
        // --- Profile Header ---
        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; } = string.Empty;

        public string? Bio { get; set; }

        // --- Preferences (Figma Dropdowns) ---
        public string Timezone { get; set; } = "UTC+6";
        public string Language { get; set; } = "English";
        public string Theme { get; set; } = "System"; // Light, Dark, System
        public string DateFormat { get; set; } = "MM/DD/YYYY";

        // --- Notification Toggles (Figma Switches) ---
        public bool StudyReminders { get; set; }
        public bool GoalAchievements { get; set; }
        public bool WeeklyReports { get; set; }
        public bool PushNotifications { get; set; }
    }
}