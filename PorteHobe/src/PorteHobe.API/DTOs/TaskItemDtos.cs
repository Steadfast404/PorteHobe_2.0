using Portehobe.src.PorteHobe.Domain.Entities;

namespace Portehobe.src.PorteHobe.API.DTOs
{
    public class TaskItemDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; } = string.Empty; // Returning as string for the frontend
        public string Status { get; set; } = string.Empty;
        public int SubjectId { get; set; }
    }

    public class CreateTaskItemDto
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public TaskPriority Priority { get; set; }
        public int SubjectId { get; set; }
    }

    public class UpdateTaskItemDto : CreateTaskItemDto
    {
        public TaskitemStatus Status { get; set; }
    }
}
