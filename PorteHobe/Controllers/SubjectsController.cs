using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portehobe.src.PorteHobe.API.DTOs;
using PorteHobe.API.DTOs;
using PorteHobe.API.Services;
using System.Security.Claims; // Needed for ClaimTypes

namespace PorteHobe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        // Add our trusty helper!
        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto dto)
        {
            try
            {
                // Pass the UserId to the service!
                var createdSubject = await _subjectService.CreateSubjectAsync(dto, GetUserId());
                return CreatedAtAction(nameof(GetSubjectById), new { id = createdSubject.Id }, createdSubject);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSubjects([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Pass the UserId!
            var subjects = await _subjectService.GetAllSubjectsAsync(pageNumber, pageSize, GetUserId());
            return Ok(subjects);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubjectById(int id)
        {
            // Pass the UserId!
            var subject = await _subjectService.GetSubjectByIdAsync(id, GetUserId());
            if (subject == null) return NotFound();

            return Ok(subject);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSubject(int id, [FromBody] UpdateSubjectDto dto)
        {
            try
            {
                // Pass the UserId!
                var updatedSubject = await _subjectService.UpdateSubjectAsync(id, dto, GetUserId());
                if (updatedSubject == null) return NotFound();

                return Ok(updatedSubject);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            // Pass the UserId!
            var success = await _subjectService.DeleteSubjectAsync(id, GetUserId());
            if (!success) return NotFound();

            return NoContent();
        }
    }
}