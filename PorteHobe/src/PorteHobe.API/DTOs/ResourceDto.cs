using System.Collections.Generic;

namespace PorteHobe.API.DTOs
{
    public class ResourceDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ResourceType { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string AuthorOrChannel { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public double Rating { get; set; }
        public string DifficultyLevel { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new List<string>();
    }
}