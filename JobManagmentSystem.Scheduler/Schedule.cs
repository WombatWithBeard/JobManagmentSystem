using System;
using JobManagmentSystem.Scheduler.Common.Enums;

namespace JobManagmentSystem.Scheduler
{
    public class Schedule
    {
        public Schedule(DateTime startDate, int intervalType, double interval)
        {
            WhenStart = GetStartJobTimeSpan(startDate);
            Period = GetPeriodJobTimeSpan(intervalType, interval);
        }

        private TimeSpan GetPeriodJobTimeSpan(int intervalType, double interval) => (IntervalsEnum) intervalType switch
        {
            IntervalsEnum.Seconds => TimeSpan.FromSeconds(interval),
            IntervalsEnum.Minutes => TimeSpan.FromMinutes(interval),
            IntervalsEnum.Hours => TimeSpan.FromHours(interval),
            IntervalsEnum.Daily => TimeSpan.FromDays(interval),
            IntervalsEnum.Monthly => TimeSpan.FromDays(interval * 30),
            IntervalsEnum.Yearly => TimeSpan.FromDays(interval * 365),
            _ => TimeSpan.Zero
        };

        private TimeSpan GetStartJobTimeSpan(DateTime startDate)
        {
            var now = DateTime.Now;
            if (now > startDate) startDate = startDate.AddDays(1);

            var timeToGo = startDate - now;
            if (timeToGo <= TimeSpan.Zero) timeToGo = TimeSpan.Zero;

            return timeToGo;
        }

        public TimeSpan Period { get; }
        public TimeSpan WhenStart { get; }
    }
}