using System;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Scheduler
{
    public class Job
    {
        public string Name { get; }
        public IJobTask Task { get; }
        public string Key { get; set; }
        public Schedule Schedule { get; }

        public Job(IJobTask task, DateTime timeStart, double interval, int intervalType, string name, string key = null)
        {
            Name = name;
            if (key == null) Key = Guid.NewGuid().ToString();
            Task = task;
            Schedule = new Schedule(timeStart, intervalType, interval);
        }
    }
}