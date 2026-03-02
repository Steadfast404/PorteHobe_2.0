using PorteHobe.API.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace PorteHobe.API.Services
{
    public class YouTubeService : IYouTubeService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        // ============================================
        // GUARDRAIL LAYER 2: Keyword Lists
        // ============================================

        private static readonly HashSet<string> BlockedKeywords = new(StringComparer.OrdinalIgnoreCase)
        {
            "movie", "film", "trailer", "music", "song", "lyrics",
            "gaming", "gameplay", "walkthrough", "playthrough",
            "vlog", "prank", "challenge", "reaction", "unboxing",
            "makeup", "beauty", "fashion", "cooking", "recipe",
            "news", "politics", "gossip", "drama", "fight",
            "tiktok", "shorts", "meme", "funny", "comedy"
        };

        private static readonly HashSet<string> AllowedSubjects = new(StringComparer.OrdinalIgnoreCase)
        {
            "Mathematics", "Physics", "Chemistry", "Computer Science",
            "Biology", "English", "History", "Economics",
            "Programming", "Data Structures", "Algorithms",
            "Calculus", "Statistics", "Geometry"
        };

        private static readonly HashSet<string> TrustedChannels = new(StringComparer.OrdinalIgnoreCase)
        {
            "3Blue1Brown", "Khan Academy", "MIT OpenCourseWare",
            "freeCodeCamp.org", "Crash Course", "Professor Leonard",
            "The Organic Chemistry Tutor", "Kurzgesagt",
            "CS Dojo", "Abdul Bari", "mycodeschool",
            "Computerphile", "Numberphile", "TED-Ed",
            "NancyPi", "PatrickJMT", "Bro Code"
        };

        private static readonly HashSet<string> EducationalIndicators = new(StringComparer.OrdinalIgnoreCase)
        {
            "tutorial", "lecture", "course", "lesson", "explained",
            "how to", "learn", "study", "education", "class",
            "chapter", "example", "solve", "proof", "theorem",
            "practice", "exercise", "beginner", "advanced",
            "introduction", "fundamentals", "basics", "guide",
            "step by step", "programming", "algorithm", "data structure",
            "physics", "chemistry", "biology", "mathematics", "calculus",
            "engineering", "science", "coding", "computer"
        };

        public YouTubeService(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _apiKey = config["YouTubeApi:ApiKey"]!;
        }

        public async Task<List<ResourceDto>> SearchVideosAsync(
            string query, string? subject = null, int maxResults = 12)
        {
            if (ContainsBlockedKeywords(query))
            {
                return new List<ResourceDto>();
            }

            var safeQuery = MakeEducational(query, subject);
            var results = await CallYouTubeApiAsync(safeQuery, maxResults);
            var filteredResults = PostFilterResults(results);

            return filteredResults;
        }

        public async Task<List<ResourceDto>> GetDefaultStudySuggestionsAsync()
        {
            var studyTopics = new List<string>
            {
                "data structures and algorithms tutorial",
                "calculus explained for beginners",
                "physics mechanics lecture",
                "organic chemistry basics",
                "linear algebra course",
                "programming fundamentals tutorial"
            };

            var allResults = new List<ResourceDto>();

            foreach (var topic in studyTopics)
            {
                var results = await CallYouTubeApiAsync(topic, 2);
                allResults.AddRange(results);
            }

            return allResults.Take(12).ToList();
        }

        private bool ContainsBlockedKeywords(string query)
        {
            var words = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return words.Any(word => BlockedKeywords.Contains(word));
        }

        private string MakeEducational(string query, string? subject)
        {
            var educationalQuery = query;

            if (!string.IsNullOrWhiteSpace(subject)
                && AllowedSubjects.Contains(subject))
            {
                educationalQuery = $"{subject} {query}";
            }

            var hasEducationalTerm = EducationalIndicators
                .Any(term => query.ToLower().Contains(term.ToLower()));

            if (!hasEducationalTerm)
            {
                educationalQuery += " tutorial";
            }

            return educationalQuery;
        }

        private List<ResourceDto> PostFilterResults(List<ResourceDto> results)
        {
            return results.Where(r =>
            {
                var titleLower = r.Title.ToLower();
                var descLower = r.Description.ToLower();
                var combined = titleLower + " " + descLower;

                if (TrustedChannels.Contains(r.AuthorOrChannel))
                    return true;

                if (EducationalIndicators.Any(term => combined.Contains(term.ToLower())))
                    return true;

                if (BlockedKeywords.Any(term => titleLower.Contains(term.ToLower())))
                    return false;

                return true;
            }).ToList();
        }

        // ============================================
        // FIXED: Removed videoCategoryId=27 filter
        // Now relies on keyword guardrails instead
        // ============================================
        private async Task<List<ResourceDto>> CallYouTubeApiAsync(
            string query, int maxResults)
        {
            var url = "https://www.googleapis.com/youtube/v3/search"
                + $"?part=snippet"
                + $"&q={Uri.EscapeDataString(query)}"
                + $"&type=video"
                + $"&safeSearch=strict"
                + $"&relevanceLanguage=en"
                + $"&maxResults={maxResults}"
                + $"&order=relevance"
                + $"&key={_apiKey}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"YouTube API Error: {response.StatusCode} - {errorContent}");
                return new List<ResourceDto>();
            }

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonDocument.Parse(content);

            var resources = new List<ResourceDto>();

            if (!json.RootElement.TryGetProperty("items", out var items))
                return resources;

            foreach (var item in items.EnumerateArray())
            {
                var snippet = item.GetProperty("snippet");
                var videoId = item.GetProperty("id")
                                  .GetProperty("videoId").GetString();

                resources.Add(new ResourceDto
                {
                    Title = snippet.GetProperty("title").GetString() ?? "",
                    Description = snippet.GetProperty("description").GetString() ?? "",
                    ResourceType = "Video",
                    AuthorOrChannel = snippet.GetProperty("channelTitle").GetString() ?? "",
                    ThumbnailUrl = snippet.GetProperty("thumbnails")
                                          .GetProperty("high")
                                          .GetProperty("url").GetString() ?? "",
                    VideoUrl = $"https://www.youtube.com/watch?v={videoId}",
                    Subject = "",
                    Duration = "",
                    ViewCount = 0,
                    Rating = 0,
                    DifficultyLevel = "",
                    Tags = new List<string>()
                });
            }

            return resources;
        }
    }
}