using Microsoft.AspNetCore.Identity;
using Portehobe.src.PorteHobe.Domain.Entities;
using PorteHobe.Domain.Entities;
using System;
using System.ComponentModel.DataAnnotations;

namespace Portehobe.Model
{
    public class AppUser : IdentityUser
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? ProfilePictureUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // --- User Preferences (Input from Figma) ---
        public string Timezone { get; set; } = "UTC+6";
        public string Language { get; set; } = "English";
        public string Theme { get; set; } = "System";
        public string DateFormat { get; set; } = "MM/DD/YYYY";

        // --- Notification Toggles ---
        public bool StudyReminders { get; set; } = true;
        public bool GoalAchievements { get; set; } = true;
        public bool WeeklyReports { get; set; } = true;
        public bool PushNotifications { get; set; } = true;
        // --- Stats ---
        public int StudyStreak { get; set; } = 0;
        //Relationships: A user can have multiple terms and todo items
        public ICollection<Term> Terms { get; set; } = new List<Term>();
        public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
        public ICollection<StudySession> StudySessions { get; set; } = new List<StudySession>();
    }
}
