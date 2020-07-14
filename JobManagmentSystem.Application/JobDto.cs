using JobManagmentSystem.Scheduler.Common.Enums;

namespace JobManagmentSystem.Application
{
    public class JobDto
    {
        public string TaskName { get; set; }
        public IntervalsEnum Interval { get; set; }
        public double TimeStart { get; set; }
        public object TaskParameters { get; set; }
    }
}