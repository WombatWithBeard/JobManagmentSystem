using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.Scheduler.Common.Models;
using Microsoft.Extensions.Logging;

namespace JobManagmentSystem.Scheduler
{
    public class Scheduler : IScheduler
    {
        private readonly ILogger<Scheduler> _logger;
        private readonly Dictionary<string, Timer> _timers;

        public Scheduler(ILogger<Scheduler> logger)
        {
            _logger = logger;
            _timers = new Dictionary<string, Timer>();
        }

        public async Task<(bool success, string message)> ScheduleJob(Job job)
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

        public async Task<(bool success, string message)> RescheduleJob(Job job)
        {
            throw new NotImplementedException();
        }

        public async Task<(bool success, string message)> UnscheduleJobById(string key)
        {
            try
            {
                if (_timers.Count <= 0) return (true, "Scheduler is empty");

                if (!_timers.ContainsKey(key)) return (true, $"Job {key} does not scheduled");
                
                await _timers.First(pair => pair.Key == key && pair.Value != null).Value.DisposeAsync();
                _timers.Remove(key);

                return (true, $"Job {key} was successfully unscheduled");

            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        public async Task<(bool success, string message)> UnscheduleAllJobs()
        {
            try
            {
                if (_timers.Count <= 0) return (true, "Scheduler is empty");

                foreach (var timersValue in _timers.Values.Where(t => t != null)) await timersValue.DisposeAsync();

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