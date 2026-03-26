using Portehobe.Model;

namespace Portehobe.src.PorteHobe.Domain.Entities
{
    public enum TaskPriority
    {
        Low = 1,
        Medium = 2,
        High = 3
    }

    public enum TaskitemStatus
    {
        Pending = 1,
        InProgress = 2,
        Completed = 3
    }

    public class TaskItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;
        public TaskitemStatus Status { get; set; } = TaskitemStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // link to Subject
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
    }
}
