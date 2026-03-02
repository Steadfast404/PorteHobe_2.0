using PorteHobe.API.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PorteHobe.API.Services
{
    public interface IYouTubeService
    {
        Task<List<ResourceDto>> SearchVideosAsync(string query, string? subject = null, int maxResults = 12);
        Task<List<ResourceDto>> GetDefaultStudySuggestionsAsync();
    }
}