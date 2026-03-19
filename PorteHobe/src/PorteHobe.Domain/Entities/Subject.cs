using PorteHobe.Domain.Common;

namespace Portehobe.Model
{
    public class Subject : BaseEntity
    {
        // Required string
        public string Name { get; set; } = string.Empty;

        // Optional, unique code (e.g., "CS101")
        public string? Code { get; set; }

        // Optional description
        public string? Description { get; set; }

        // --- Relationships (To be uncommented in Phase 2 & 3) ---
        // public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
        // public ICollection<StudySession> StudySessions { get; set; } = new List<StudySession>();
    }
}
