namespace PorteHobe.API.DTOs
{
    public class ResourceSearchRequestDto
    {
        public string? SearchQuery { get; set; }
        public string? Subject { get; set; }
        public string? ResourceType { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
    }
}