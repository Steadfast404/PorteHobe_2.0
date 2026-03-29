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
    }
}