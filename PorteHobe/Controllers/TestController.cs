using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Portehobe.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // Protected endpoint, but no role restriction
        [HttpGet("student-only")]
        [Authorize] // Only requires authentication, no roles
        public IActionResult StudentOnly()
        {
            return Ok("Welcome Authenticated User");
        }

        // Protected endpoint, but no role restriction
        [HttpGet("admin-only")]
        [Authorize] // Only requires authentication, no roles
        public IActionResult AdminOnly()
        {
            return Ok("Welcome Authenticated User");
        }

        // Fully public endpoint
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("Public endpoint");
        }
    }
}