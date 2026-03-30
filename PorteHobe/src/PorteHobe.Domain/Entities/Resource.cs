using PorteHobe.Domain.Common;

namespace PorteHobe.Domain.Entities
{
    public class Resource : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ResourceType { get; set; } = "Video";
        public string Subject { get; set; } = string.Empty;
        public string AuthorOrChannel { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public double Rating { get; set; }
        public string DifficultyLevel { get; set; } = "Beginner";
        public string Tags { get; set; } = string.Empty;
        public bool IsFeatured { get; set; } = false;
    }
}