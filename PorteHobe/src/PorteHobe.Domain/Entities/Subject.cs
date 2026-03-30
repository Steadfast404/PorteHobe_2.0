using Portehobe.src.PorteHobe.Domain.Entities;
using PorteHobe.Domain.Common;
using PorteHobe.Domain.Entities;

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
        //Link to Term
        public int TermId { get; set; }
        public Term Term { get; set; }

        // --- Relationships: A subject may have multiple tasks and study sessions ---
        public ICollection<TaskItem> TaskItems { get; set; } = new List<TaskItem>();
        //study session
        public ICollection<StudySession> StudySessions { get; set; } = new List<StudySession>();
    }
}
