
using PorteHobe.Domain.Common;
namespace PorteHobe.Domain.Entities
{
    public class Term : BaseEntity
    {
        public string UserId { get; set; } 
        public string EducationLevel { get; set; } // University or School
        public string? Semester { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TermName { get; set; }
    }
}