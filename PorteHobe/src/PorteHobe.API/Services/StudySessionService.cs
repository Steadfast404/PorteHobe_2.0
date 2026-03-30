using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PorteHobe.API.DTOs;
using PorteHobe.Domain.Entities;
using Portehobe.Model;
using Portehobe.src.PorteHobe.Domain.Entities;
using Portehobe.src.PorteHobe.Infrastructure;

namespace PorteHobe.API.Services
{
    public class StudySessionService : IStudySessionService
    {
        private readonly AppDbContext _context;

        public StudySessionService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StudySessionDto> StartSessionAsync(string userId, StartStudySessionDto dto)
        {
            var subject = await _context.Subjects
                .FirstOrDefaultAsync(s => s.Id == dto.SubjectId);

            if (subject == null)
                throw new InvalidOperationException("Subject not found.");

            TaskItem? task = null;
            if (dto.TaskItemId.HasValue)
            {
                task = await _context.TaskItems
                    .FirstOrDefaultAsync(t =>
                        t.Id == dto.TaskItemId.Value &&
                        t.SubjectId == dto.SubjectId);

                if (task == null)
                    throw new InvalidOperationException("Task not found for this subject.");
            }

            var hasActive = await _context.StudySessions
                .AnyAsync(s => s.AppUserId == userId && s.EndTime == null);

            if (hasActive)
                throw new InvalidOperationException("You already have an active study session.");

            var session = new StudySession
            {
                AppUserId = userId,
                SubjectId = dto.SubjectId,
                TaskItemId = dto.TaskItemId,
                StartTime = DateTime.UtcNow,
                EndTime = null
            };

            _context.StudySessions.Add(session);
            await _context.SaveChangesAsync();

            return new StudySessionDto
            {
                Id = session.Id,
                SubjectId = session.SubjectId,
                TaskItemId = session.TaskItemId,
                StartTime = session.StartTime,
                EndTime = session.EndTime
            };
        }

        public async Task<bool> StopSessionAsync(string userId, int sessionId)
        {
            var session = await _context.StudySessions
                .FirstOrDefaultAsync(s => s.Id == sessionId && s.AppUserId == userId);

            if (session == null || session.EndTime.HasValue)
                return false;

            session.EndTime = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<IEnumerable<StudySessionDto>> GetUserSessionsAsync(string userId)
        {
            return await _context.StudySessions
                .Where(s => s.AppUserId == userId)
                .OrderByDescending(s => s.StartTime)
                .Select(s => new StudySessionDto
                {
                    Id = s.Id,
                    SubjectId = s.SubjectId,
                    TaskItemId = s.TaskItemId,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime
                })
                .ToListAsync();
        }

        public async Task<TotalTimeDto> GetTotalTimeForSubjectAsync(string userId, int subjectId)
        {
            var sessions = await _context.StudySessions
                .Where(s => s.AppUserId == userId &&
                            s.SubjectId == subjectId &&
                            s.EndTime != null)
                .ToListAsync();

            var totalSeconds = sessions.Sum(s => (s.EndTime!.Value - s.StartTime).TotalSeconds);

            return new TotalTimeDto
            {
                ReferenceId = subjectId,
                TotalSeconds = totalSeconds
            };
        }

        public async Task<TotalTimeDto> GetTotalTimeForTaskAsync(string userId, int taskItemId)
        {
            var sessions = await _context.StudySessions
                .Where(s => s.AppUserId == userId &&
                            s.TaskItemId == taskItemId &&
                            s.EndTime != null)
                .ToListAsync();

            var totalSeconds = sessions.Sum(s => (s.EndTime!.Value - s.StartTime).TotalSeconds);

            return new TotalTimeDto
            {
                ReferenceId = taskItemId,
                TotalSeconds = totalSeconds
            };
        }

        public async Task<StudyStreakSummaryDto> GetStreakSummaryAsync(
    string userId,
    DateOnly fromDate,
    DateOnly toDate,
    int minDailySeconds = 600)
        {
            // Convert DateOnly range to DateTime (UTC) range
            var fromDateTime = fromDate.ToDateTime(TimeOnly.MinValue, DateTimeKind.Utc);
            var toDateTime = toDate.ToDateTime(TimeOnly.MaxValue, DateTimeKind.Utc);

            // Load completed sessions within the range
            var sessions = await _context.StudySessions
                .Where(s => s.AppUserId == userId
                            && s.EndTime != null
                            && s.StartTime >= fromDateTime
                            && s.StartTime <= toDateTime)
                .ToListAsync();

            // Aggregate totals per day
            var totalsByDate = new Dictionary<DateOnly, double>();

            foreach (var s in sessions)
            {
                // For now we treat StartTime as UTC and use its date.
                var date = DateOnly.FromDateTime(s.StartTime.Date);
                var durationSeconds = (s.EndTime!.Value - s.StartTime).TotalSeconds;

                if (totalsByDate.ContainsKey(date))
                    totalsByDate[date] += durationSeconds;
                else
                    totalsByDate[date] = durationSeconds;
            }

            // Build daily summaries for every day in the range
            var dailySummaries = new List<DailyStudySummaryDto>();
            var current = fromDate;
            while (current <= toDate)
            {
                totalsByDate.TryGetValue(current, out var totalSeconds);

                dailySummaries.Add(new DailyStudySummaryDto
                {
                    Date = current,
                    TotalSeconds = totalSeconds,
                    IsStudyDay = totalSeconds >= minDailySeconds
                });

                current = current.AddDays(1);
            }

            // Compute today & last 7 days totals
            var today = toDate; // by design, toDate is typically "today"
            var todaySummary = dailySummaries.FirstOrDefault(d => d.Date == today);
            var todayTotalSeconds = todaySummary?.TotalSeconds ?? 0.0;

            var last7DaysTotalSeconds = dailySummaries
                .Where(d => d.Date >= today.AddDays(-6) && d.Date <= today)
                .Sum(d => d.TotalSeconds);

            // Compute current streak (consecutive study days ending at "today")
            int currentStreak = 0;
            var streakDate = today;

            while (streakDate >= fromDate)
            {
                var day = dailySummaries.FirstOrDefault(d => d.Date == streakDate);
                if (day == null || !day.IsStudyDay)
                    break;

                currentStreak++;
                streakDate = streakDate.AddDays(-1);
            }

            // Compute longest streak in the whole range
            int longestStreak = 0;
            int runningStreak = 0;

            foreach (var day in dailySummaries.OrderBy(d => d.Date))
            {
                if (day.IsStudyDay)
                {
                    runningStreak++;
                    if (runningStreak > longestStreak)
                        longestStreak = runningStreak;
                }
                else
                {
                    runningStreak = 0;
                }
            }

            return new StudyStreakSummaryDto
            {
                CurrentStreakDays = currentStreak,
                LongestStreakDays = longestStreak,
                TodayTotalSeconds = todayTotalSeconds,
                Last7DaysTotalSeconds = last7DaysTotalSeconds,
                DailySummaries = dailySummaries
            };
        }
    }
}