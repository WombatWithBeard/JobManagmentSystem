using System;
using JobManagmentSystem.Scheduler.Common.Enums;

namespace JobManagmentSystem.Scheduler.Common.Models
{
    public class Schedule
    {
        public DateTime StartDate { get; set; }
        public int IntervalType { get; set; }
        public double Interval { get; set; }

        public Schedule()
        {
        }

        public Schedule(DateTime startDate, int intervalType, double interval)
        {
            StartDate = startDate;
            IntervalType = intervalType;
            Interval = interval;
        }

        public TimeSpan GetPeriodJobTimeSpan() => (IntervalsEnum) IntervalType switch
        {
            IntervalsEnum.Seconds => TimeSpan.FromSeconds(Interval),
            IntervalsEnum.Minutes => TimeSpan.FromMinutes(Interval),
            IntervalsEnum.Hours => TimeSpan.FromHours(Interval),
            IntervalsEnum.Daily => TimeSpan.FromDays(Interval),
            IntervalsEnum.Monthly => TimeSpan.FromDays(Interval * 30),
            IntervalsEnum.Yearly => TimeSpan.FromDays(Interval * 365),
            _ => TimeSpan.Zero
        };

        public TimeSpan GetStartJobTimeSpan()
        {
            var now = DateTime.Now;
            while (now > StartDate) StartDate = StartDate.AddDays(1);

            var timeToGo = StartDate - now;
            if (timeToGo <= TimeSpan.Zero) timeToGo = TimeSpan.Zero;

            return timeToGo;
        }
    }
}