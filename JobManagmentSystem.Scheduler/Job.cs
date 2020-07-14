using System;
using System.Threading;
using JobManagmentSystem.Scheduler.Common.Enums;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace JobManagmentSystem.Scheduler
{
    public class Job
    {
        private IJobTask Task { get; }
        public string Key { get; }
        public Timer Timer { get; }
        private Schedule Schedule { get; }

        public Job(IJobTask task, in double dtoTimeStart, int dtoInterval)
        {
            Key = Guid.NewGuid().ToString();
            Task = task;
            Schedule = new Schedule(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(15));
            // Schedule = new Schedule(TimeSpan.FromSeconds(dtoTimeStart), TimeSpan.FromSeconds(dtoInterval));
            Timer = new Scheduler().Schedule(Task, Schedule);
        }
    }
}