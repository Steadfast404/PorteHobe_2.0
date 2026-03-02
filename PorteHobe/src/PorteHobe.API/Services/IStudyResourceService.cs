using PorteHobe.API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PorteHobe.API.Services
{
    public interface IStudyResourceService
    {
        Task<List<ResourceDto>> GetDefaultSuggestionsAsync();
        Task<List<ResourceDto>> SearchResourcesAsync(ResourceSearchRequestDto request);
        Task<List<ResourceDto>> GetBySubjectAsync(string subject);
    }
}