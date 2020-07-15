using System;
using System.Threading;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Scheduler
{
    public class Job
    {
        public IJobTask Task { get; }
        public string Key { get; }
        public Schedule Schedule { get; }

        public Job(IJobTask task, in double dtoTimeStart, int dtoInterval)
        {
            Key = Guid.NewGuid().ToString();
            Task = task;
            Schedule = new Schedule(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(15)); //TODO:
            // Schedule = new Schedule(TimeSpan.FromSeconds(dtoTimeStart), TimeSpan.FromSeconds(dtoInterval));
        }
    }
}