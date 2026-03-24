using Microsoft.AspNetCore.Mvc;
using Portehobe.src.PorteHobe.API.DTOs;
using Portehobe.src.PorteHobe.API.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Portehobe.src.PorteHobe.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoItemService _todoService;

        public TodoItemsController(ITodoItemService todoService)
        {
            _todoService = todoService;
        }

        private string GetUserId()
        {
            // Dummy ID until authentication is ready
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "dev-test-user-001";
        }

        [HttpGet]
        public async Task<IActionResult> GetMyTodos()
        {
            var todos = await _todoService.GetUserTodosAsync(GetUserId());
            return Ok(todos);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTodo([FromBody] CreateTodoItemDto dto)
        {
            var todo = await _todoService.CreateTodoAsync(dto, GetUserId());
            return Ok(todo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTodo(int id, [FromBody] UpdateTodoItemDto dto)
        {
            var success = await _todoService.UpdateTodoAsync(id, dto, GetUserId());
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            var success = await _todoService.DeleteTodoAsync(id, GetUserId());
            if (!success) return NotFound();
            return NoContent();
        }
    }
}