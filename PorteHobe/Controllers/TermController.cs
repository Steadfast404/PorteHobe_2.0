using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portehobe.Model; // Context namespace
using PorteHobe.API.DTOs;
using PorteHobe.Domain.Entities;
using System.Security.Claims;
using System.Threading.Tasks;
using Portehobe.src.PorteHobe.Infrastructure;// For AppDbContext

namespace PorteHobe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TermController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TermController(AppDbContext context)
        {
            _context = context;
        }

        // 1. POST: api/Term/setup (Term create korar jonno)
        [HttpPost("setup")]
        public async Task<IActionResult> SetupTerm([FromBody] TermSetupDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            string termName = dto.EducationLevel == "University"
                ? $"{dto.Semester} {dto.Year}"
                : $"{dto.Year} Academic Year";

            var term = new Term
            {
                AppUserId = userId,
                EducationLevel = dto.EducationLevel,
                Semester = dto.Semester,
                Year = dto.Year,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TermName = termName
            };

            _context.Terms.Add(term);
            await _context.SaveChangesAsync();

            var totalDays = (dto.EndDate - dto.StartDate).TotalDays;

            return Ok(new
            {
                Message = "Success",
                TermName = termName,
                TotalDays = (int)totalDays,
                User = userId
            });
        }

        // 2. GET: api/Term/my-term (Postman-e dashboard data dekhar jonno)
        [HttpGet("my-term")]
        public async Task<IActionResult> GetMyTerm()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var term = await _context.Terms
                .Where(t => t.AppUserId == userId)
                .OrderByDescending(t => t.Id)
                .FirstOrDefaultAsync();

            if (term == null) return NotFound(new { message = "No term setup found." });

            // --- Dashboard Calculation Logic ---
            var today = DateTime.UtcNow;
            var totalDays = (term.EndDate - term.StartDate).TotalDays;
            var daysRemaining = (term.EndDate - today).TotalDays;
            var daysPassed = (today - term.StartDate).TotalDays;

            // Progress percentage calculation
            double progress = totalDays > 0 ? (daysPassed / totalDays) * 100 : 0;

            return Ok(new
            {
                termName = term.TermName,
                educationLevel = term.EducationLevel,
                semester = term.Semester,
                year = term.Year,
                startDate = term.StartDate.ToString("yyyy-MM-dd"),
                endDate = term.EndDate.ToString("yyyy-MM-dd"),
                totalDays = Math.Max(0, (int)totalDays),
                daysRemaining = Math.Max(0, (int)daysRemaining),
                progressPercentage = Math.Clamp(Math.Round(progress, 2), 0, 100),
                status = daysRemaining > 0 ? "Active" : "Completed"
            });
        }
    }
}