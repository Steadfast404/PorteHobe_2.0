using Portehobe.Model;

namespace Portehobe.src.PorteHobe.Domain.Entities
{
    public class TodoItem
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsDone { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Linked ONLY to the user
        //public string UserId { get; set; } = string.Empty;
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
    }
}
