// src/PorteHobe.API/DTOs/StudyStreakDtos.cs
using System;
using System.Collections.Generic;

namespace PorteHobe.API.DTOs
{
    public class DailyStudySummaryDto
    {
        public DateOnly Date { get; set; }
        public double TotalSeconds { get; set; }
        public bool IsStudyDay { get; set; }
    }

    public class StudyStreakSummaryDto
    {
        public int CurrentStreakDays { get; set; }
        public int LongestStreakDays { get; set; }

        public double TodayTotalSeconds { get; set; }
        public double Last7DaysTotalSeconds { get; set; }

        // For the heatmap: one entry per day in the requested range
        public List<DailyStudySummaryDto> DailySummaries { get; set; } = new();
    }
}