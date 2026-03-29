using System;

namespace PorteHobe.API.DTOs
{
    public class StartStudySessionDto
    {
        public int SubjectId { get; set; }
        public int? TaskItemId { get; set; }
    }

    public class StudySessionDto
    {
        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int? TaskItemId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public double? DurationSeconds =>
            EndTime.HasValue ? (EndTime.Value - StartTime).TotalSeconds : (double?)null;
    }

    public class TotalTimeDto
    {
        public int ReferenceId { get; set; }   // Subject or TaskItem Id
        public double TotalSeconds { get; set; }
    }
}