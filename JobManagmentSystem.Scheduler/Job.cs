using System;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Scheduler
{
    public class Job
    {
        private readonly double _dtoTimeStart;
        private readonly int _dtoInterval;
        public string Name { get; }
        public IJobTask Task { get; }
        public string Key { get; set; }
        public Schedule Schedule { get; }

        public Job(IJobTask task, double dtoTimeStart, int dtoInterval, string name, string key = null)
        {
            Name = name;
            if (key == null) Key = Guid.NewGuid().ToString();
            Task = task;
            Schedule = new Schedule(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(15)); //TODO:
            // Schedule = new Schedule(TimeSpan.FromSeconds(dtoTimeStart), TimeSpan.FromSeconds(dtoInterval));
        }
    }
}