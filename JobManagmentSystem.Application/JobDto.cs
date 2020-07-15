using System;
using JobManagmentSystem.Scheduler.Common.Enums;

namespace JobManagmentSystem.Application
{
    public class JobDto
    {
        public string TaskName { get; set; }
        public int IntervalType { get; set; }
        public double Interval { get; set; }
        public string TimeStart { get; set; }
        public object TaskParameters { get; set; }
    }
}