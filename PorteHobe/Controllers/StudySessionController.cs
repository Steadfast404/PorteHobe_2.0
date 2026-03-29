using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PorteHobe.API.DTOs;
using PorteHobe.API.Services;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PorteHobe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class StudySessionsController : ControllerBase
    {
        private readonly IStudySessionService _studySessionService;

        public StudySessionsController(IStudySessionService studySessionService)
        {
            _studySessionService = studySessionService;
        }

        private string GetUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartSession([FromBody] StartStudySessionDto dto)
        {
            try
            {
                var session = await _studySessionService.StartSessionAsync(GetUserId(), dto);
                return Ok(session);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{id}/stop")]
        public async Task<IActionResult> StopSession(int id)
        {
            var success = await _studySessionService.StopSessionAsync(GetUserId(), id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetMySessions()
        {
            var sessions = await _studySessionService.GetUserSessionsAsync(GetUserId());
            return Ok(sessions);
        }

        [HttpGet("subject/{subjectId}/total-time")]
        public async Task<IActionResult> GetSubjectTotalTime(int subjectId)
        {
            var total = await _studySessionService.GetTotalTimeForSubjectAsync(GetUserId(), subjectId);
            return Ok(total);
        }

        [HttpGet("task/{taskItemId}/total-time")]
        public async Task<IActionResult> GetTaskTotalTime(int taskItemId)
        {
            var total = await _studySessionService.GetTotalTimeForTaskAsync(GetUserId(), taskItemId);
            return Ok(total);
        }
    }
}