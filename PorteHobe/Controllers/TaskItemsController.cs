using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Portehobe.src.PorteHobe.API.DTOs;
using Portehobe.src.PorteHobe.API.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PorteHobe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // authentication is required for all endpoints in this controller
    public class TaskItemsController : ControllerBase
    {
        private readonly ITaskItemService _taskService;

        public TaskItemsController(ITaskItemService taskService)
        {
            _taskService = taskService;
        }

        private string GetUserId()
        {
            // For now, returning a dummy ID if no user is logged in
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "dev-test-user-001";
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTasks()
        {
            var tasks = await _taskService.GetUserTasksAsync(GetUserId());
            return Ok(tasks);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var task = await _taskService.GetTaskByIdAsync(id, GetUserId());
            if (task == null) return NotFound();
            return Ok(task);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTask([FromBody] CreateTaskItemDto dto)
        {
            var task = await _taskService.CreateTaskAsync(dto, GetUserId());
            return CreatedAtAction(nameof(GetTask), new { id = task.Id }, task);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTask(int id, [FromBody] UpdateTaskItemDto dto)
        {
            var success = await _taskService.UpdateTaskAsync(id, dto, GetUserId());
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var success = await _taskService.DeleteTaskAsync(id, GetUserId());
            if (!success) return NotFound();
            return NoContent();
        }
    }
}