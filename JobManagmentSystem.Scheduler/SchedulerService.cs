using System;
using System.Collections.Generic;
using System.Threading;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobManagmentSystem.Scheduler
{
    public class SchedulerService : IScheduler
    {
        private readonly ILogger<SchedulerService> _logger;
        private readonly Dictionary<string, Timer> _timers;

        public SchedulerService(ILogger<SchedulerService> logger)
        {
            _logger = logger;
            _timers = new Dictionary<string, Timer>();
        }

        public (bool, string) AddJob(Job job)
        {
            try
            {
                var timer = new Timer(s => job.Task.Invoke(s), job.Task, job.Schedule.WhenStart, job.Schedule.Period);
                _timers.Add(job.Key, timer);
                return (true, $"Job {job.Key} was successfully scheduled");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        public (bool, string) DeleteJobById(string key)
        {
            try
            {
                _timers.Remove(key);
                return (true, $"Job {key} was successfully unscheduled");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        public (bool, string) DeleteAllJobs()
        {
            try
            {
                _timers.Clear();
                return (true, "All job was successfully unscheduled");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }
    }
}