using System;

namespace JobManagmentSystem.Scheduler
{
    public class Schedule
    {
        public Schedule(TimeSpan fromSeconds, TimeSpan timeSpan)
        {
            WhenStart = fromSeconds;
            Period = timeSpan;
        }

        public TimeSpan Period { get; }
        public TimeSpan WhenStart { get; }
    }
}