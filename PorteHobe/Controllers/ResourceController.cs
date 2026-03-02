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
        private readonly IStudyResourceService _resourceService;
        private readonly IYouTubeService _youtubeService;

        public ResourcesController(
            IStudyResourceService resourceService,
            IYouTubeService youtubeService)
        {
            _resourceService = resourceService;
            _youtubeService = youtubeService;
        }

        // ==========================================
        // DATABASE ENDPOINTS (existing)
        // ==========================================

        // GET: api/resources/suggestions
        [HttpGet("suggestions")]
        public async Task<ActionResult<List<ResourceDto>>> GetSuggestions()
        {
            var resources = await _resourceService.GetDefaultSuggestionsAsync();
            return Ok(resources);
        }

        // GET: api/resources/search?searchQuery=merge+sort
        [HttpGet("search")]
        public async Task<ActionResult<List<ResourceDto>>> Search(
            [FromQuery] ResourceSearchRequestDto request)
        {
            var resources = await _resourceService.SearchResourcesAsync(request);
            return Ok(resources);
        }

        // GET: api/resources/subject/Mathematics
        [HttpGet("subject/{subject}")]
        public async Task<ActionResult<List<ResourceDto>>> GetBySubject(string subject)
        {
            var resources = await _resourceService.GetBySubjectAsync(subject);
            return Ok(resources);
        }

        // ==========================================
        // YOUTUBE ENDPOINTS (new — with guardrails)
        // ==========================================

        // GET: api/resources/youtube/suggestions
        // Page loads → show real YouTube study suggestions
        [HttpGet("youtube/suggestions")]
        public async Task<ActionResult<List<ResourceDto>>> GetYouTubeSuggestions()
        {
            var videos = await _youtubeService.GetDefaultStudySuggestionsAsync();
            return Ok(videos);
        }

        // GET: api/resources/youtube/search?query=binary+tree&subject=Computer+Science
        // User searches → get YouTube educational videos
        [HttpGet("youtube/search")]
        public async Task<ActionResult<List<ResourceDto>>> SearchYouTube(
            [FromQuery] string query,
            [FromQuery] string? subject = null)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query cannot be empty.");

            var videos = await _youtubeService.SearchVideosAsync(query, subject);

            if (videos.Count == 0)
                return Ok(new
                {
                    message = "No educational videos found. Please search for study-related topics.",
                    results = videos
                });

            return Ok(videos);
        }
    }
}