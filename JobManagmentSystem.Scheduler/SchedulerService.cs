using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.Scheduler.Common.Models;
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
                if (_timers.ContainsKey(job.Key)) return (false, $"Job {job.Key} already exists");

                var timer = new Timer(s => job.Task.Invoke(s),
                    job.Task,
                    job.Schedule.GetStartJobTimeSpan(),
                    job.Schedule.GetPeriodJobTimeSpan());
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
                if (_timers.Count <= 0) return (true, "Scheduler is empty");

                if (_timers.ContainsKey(key))
                {
                    _timers.First(t => t.Key == key).Value?.Dispose();
                    _timers.Remove(key);

                    return (true, $"Job {key} was successfully unscheduled");
                }

                return (true, "Job does not exist");
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
                if (_timers.Count <= 0) return (true, "Scheduler is empty");

                foreach (var timersValue in _timers.Values) timersValue?.Dispose();

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