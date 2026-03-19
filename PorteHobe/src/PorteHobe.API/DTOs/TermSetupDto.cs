

namespace PorteHobe.API.DTOs
{
    public class TermSetupDto
    {
        public string EducationLevel { get; set; }
        public string? Semester { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}