using Microsoft.AspNetCore.Mvc;
using PorteHobe.Domain.Entities;
using PorteHobe.API.DTOs;
using Portehobe.Model; // AppDbContext er namespace

namespace PorteHobe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TermController : ControllerBase
    {
        private readonly AppDbContext _context; // Nam change hoyeche

        public TermController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("setup")]
        public async Task<IActionResult> SetupTerm([FromBody] TermSetupDto dto)
        {
            string termName = dto.EducationLevel == "University"
                ? $"{dto.Semester} {dto.Year}"
                : $"{dto.Year} Academic Year";

            var term = new Term
            {
                EducationLevel = dto.EducationLevel,
                Semester = dto.Semester,
                Year = dto.Year,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TermName = termName
            };

            _context.Terms.Add(term);
            await _context.SaveChangesAsync();

            var days = (dto.EndDate - dto.StartDate).TotalDays;
            return Ok(new { Message = "Success", TermName = termName, TotalDays = days });
        }
    }
}