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

        public async Task<(bool success, string message)> ScheduleJobAsync(Job job)
        {
            try
            {
                if (_timers.ContainsKey(job.Key)) return (false, $"Job {job.Key} already exists");

                var timer = CreateNewTimer(job);

                var addResult = _timers.TryAdd(job.Key, timer);

                return addResult
                    ? (true, $"Job {job.Key} was successfully scheduled")
                    : (false, $"Job {job.Key} schedule failed");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        public async Task<(bool success, string message)> UnscheduleJobAsync(string key)
        {
            try
            {
                if (string.IsNullOrEmpty(key)) return (false, "Key is empty");

                if (_timers.Count <= 0) return (true, "Scheduler is empty");

                if (!_timers.ContainsKey(key)) return (true, $"Job {key} does not scheduled");

                await _timers.First(pair => pair.Key == key && pair.Value != null).Value.DisposeAsync();

                var removeResult = _timers.Remove(key);

                return removeResult
                    ? (true, $"Job {key} was successfully unscheduled")
                    : (false, $"Remove job {key} from scheduler was failed");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        public async Task<(bool success, string message)> RescheduleJobAsync(Job job)
        {
            try
            {
                if (_timers.Count > 0 && _timers.ContainsKey(job.Key))
                {
                    var removeResult = _timers.Remove(job.Key);
                    if (!removeResult) return (false, $"Remove job {job.Key} from scheduler was failed");
                }

                var timer = CreateNewTimer(job);

                var addResult = _timers.TryAdd(job.Key, timer);

                return addResult
                    ? (true, $"Job {job.Key} was successfully scheduled")
                    : (false, $"Job {job.Key} schedule failed");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        public async Task<(bool success, string message, string job)> GetJobAsync(string key)
        {
            try
            {
                if (_timers.Count <= 0) return (false, "Scheduler is empty", null);

                if (_timers.ContainsKey(key)) return (true, $"Job: {key} is active", null);

                return (false, $"Job: {key} is not scheduled", null);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        private Timer CreateNewTimer(Job job) => new Timer(
            s => job.Task.Invoke(s),
            job.TaskParameters,
            job.Schedule.GetStartJobTimeSpan(),
            job.Schedule.GetPeriodJobTimeSpan());
    }
}