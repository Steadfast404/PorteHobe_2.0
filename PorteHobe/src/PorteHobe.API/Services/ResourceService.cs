using Microsoft.EntityFrameworkCore;
using Portehobe.Model;
using PorteHobe.API.DTOs;
using PorteHobe.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PorteHobe.API.Services
{
    public class ResourceService : IStudyResourceService  // ← changed
    {
        private readonly AppDbContext _context;

        public ResourceService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ResourceDto>> GetDefaultSuggestionsAsync()
        {
            var resources = await _context.Resources
                .Where(r => r.IsFeatured == true)
                .OrderByDescending(r => r.Rating)
                .Take(12)
                .ToListAsync();

            return resources.Select(r => MapToDto(r)).ToList();
        }

        public async Task<List<ResourceDto>> SearchResourcesAsync(ResourceSearchRequestDto request)
        {
            var query = _context.Resources.AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.SearchQuery))
            {
                var searchTerm = request.SearchQuery.ToLower();
                query = query.Where(r =>
                    r.Title.ToLower().Contains(searchTerm) ||
                    r.Description.ToLower().Contains(searchTerm) ||
                    r.Tags.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(request.Subject)
                && request.Subject != "All Subjects")
            {
                query = query.Where(r => r.Subject == request.Subject);
            }

            if (!string.IsNullOrWhiteSpace(request.ResourceType)
                && request.ResourceType != "All Resources")
            {
                query = query.Where(r => r.ResourceType == request.ResourceType);
            }

            var resources = await query
                .OrderByDescending(r => r.Rating)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return resources.Select(r => MapToDto(r)).ToList();
        }

        public async Task<List<ResourceDto>> GetBySubjectAsync(string subject)
        {
            var resources = await _context.Resources
                .Where(r => r.Subject == subject)
                .OrderByDescending(r => r.Rating)
                .ToListAsync();

            return resources.Select(r => MapToDto(r)).ToList();
        }

        private ResourceDto MapToDto(Resource resource)
        {
            return new ResourceDto
            {
                Id = resource.Id,
                Title = resource.Title,
                Description = resource.Description,
                ResourceType = resource.ResourceType,
                Subject = resource.Subject,
                AuthorOrChannel = resource.AuthorOrChannel,
                ThumbnailUrl = resource.ThumbnailUrl,
                VideoUrl = resource.VideoUrl,
                Duration = resource.Duration,
                ViewCount = resource.ViewCount,
                Rating = resource.Rating,
                DifficultyLevel = resource.DifficultyLevel,
                Tags = string.IsNullOrEmpty(resource.Tags)
                    ? new List<string>()
                    : resource.Tags.Split(',').Select(t => t.Trim()).ToList()
            };
        }
    }
}