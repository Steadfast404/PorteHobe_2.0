namespace Portehobe.src.PorteHobe.API.DTOs
{
    public class TodoItemDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsDone { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTodoItemDto
    {
        public string Description { get; set; } = string.Empty;
    }

    public class UpdateTodoItemDto
    {
        public string Description { get; set; } = string.Empty;
        public bool IsDone { get; set; }
    }
}
