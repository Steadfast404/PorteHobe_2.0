using Microsoft.AspNetCore.Mvc;
using Portehobe.src.PorteHobe.API.DTOs;
using PorteHobe.API.DTOs;
using PorteHobe.API.Services;

namespace PorteHobe.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SubjectsController : ControllerBase
{
    private readonly ISubjectService _subjectService;

    public SubjectsController(ISubjectService subjectService)
    {
        _subjectService = subjectService;
    }

    // POST: /api/subjects
    [HttpPost]
    public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto dto)
    {
        try
        {
            var createdSubject = await _subjectService.CreateSubjectAsync(dto);
            return CreatedAtAction(nameof(GetSubjectById), new { id = createdSubject.Id }, createdSubject);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message }); // Handles duplicate code error
        }
    }

    // GET: /api/subjects?pageNumber=1&pageSize=10
    [HttpGet]
    public async Task<IActionResult> GetAllSubjects([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {
        var subjects = await _subjectService.GetAllSubjectsAsync(pageNumber, pageSize);
        return Ok(subjects);
    }

    // GET: /api/subjects/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetSubjectById(int id)
    {
        var subject = await _subjectService.GetSubjectByIdAsync(id);
        if (subject == null) return NotFound();

        return Ok(subject);
    }

    // PUT: /api/subjects/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSubject(int id, [FromBody] UpdateSubjectDto dto)
    {
        try
        {
            var updatedSubject = await _subjectService.UpdateSubjectAsync(id, dto);
            if (updatedSubject == null) return NotFound();

            return Ok(updatedSubject);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: /api/subjects/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSubject(int id)
    {
        var success = await _subjectService.DeleteSubjectAsync(id);
        if (!success) return NotFound();

        return NoContent(); // 204 No Content is standard for successful deletions
    }
}