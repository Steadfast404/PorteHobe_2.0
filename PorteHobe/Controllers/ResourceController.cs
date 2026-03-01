using Microsoft.AspNetCore.Mvc;
using PorteHobe.API.DTOs;
using PorteHobe.API.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PorteHobe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ResourcesController : ControllerBase
    {
        private readonly IStudyResourceService _resourceService;  // ← changed

        public ResourcesController(IStudyResourceService resourceService)  // ← changed
        {
            _resourceService = resourceService;
        }

        [HttpGet("suggestions")]
        public async Task<ActionResult<List<ResourceDto>>> GetSuggestions()
        {
            var resources = await _resourceService.GetDefaultSuggestionsAsync();
            return Ok(resources);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<ResourceDto>>> Search(
            [FromQuery] ResourceSearchRequestDto request)
        {
            var resources = await _resourceService.SearchResourcesAsync(request);
            return Ok(resources);
        }

        [HttpGet("subject/{subject}")]
        public async Task<ActionResult<List<ResourceDto>>> GetBySubject(string subject)
        {
            var resources = await _resourceService.GetBySubjectAsync(subject);
            return Ok(resources);
        }
    }
}