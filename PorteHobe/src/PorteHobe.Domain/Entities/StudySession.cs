using Portehobe.Model;
using Portehobe.src.PorteHobe.Domain.Entities;
using System;

namespace PorteHobe.Domain.Entities
{
    public class StudySession
    {
        public int Id { get; set; }

        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;

        public int? TaskItemId { get; set; }
        public TaskItem? TaskItem { get; set; }

        public DateTime StartTime { get; set; } = DateTime.UtcNow;
        public DateTime? EndTime { get; set; }
    }
}