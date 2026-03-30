namespace Portehobe.API.DTOs
{
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string? Token { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }
    }
}
