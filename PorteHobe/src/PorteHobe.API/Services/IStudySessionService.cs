using System.Collections.Generic;
using System.Threading.Tasks;
using PorteHobe.API.DTOs;

namespace PorteHobe.API.Services
{
    public interface IStudySessionService
    {
        Task<StudySessionDto> StartSessionAsync(string userId, StartStudySessionDto dto);
        Task<bool> StopSessionAsync(string userId, int sessionId);
        Task<IEnumerable<StudySessionDto>> GetUserSessionsAsync(string userId);
        Task<TotalTimeDto> GetTotalTimeForSubjectAsync(string userId, int subjectId);
        Task<TotalTimeDto> GetTotalTimeForTaskAsync(string userId, int taskItemId);

        Task<StudyStreakSummaryDto> GetStreakSummaryAsync(
           string userId,
           DateOnly fromDate,
           DateOnly toDate,
           int minDailySeconds = 600); // 10 minutes default
    }
}