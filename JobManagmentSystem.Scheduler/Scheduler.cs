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
        private readonly Dictionary<string, (Timer, Job)> _timers;

        public Scheduler(ILogger<Scheduler> logger)
        {
            _logger = logger;
            _timers = new Dictionary<string, (Timer, Job)>();
        }

        public async Task<(bool success, string message)> ScheduleJobAsync(Job job)
        {
            if (_timers.ContainsKey(job.Key)) return (false, $"Job {job.Key} already exists");

            var timer = CreateNewTimer(job);

            var addResult = _timers.TryAdd(job.Key, (timer, job));

            return addResult
                ? (true, $"Job {job.Key} was successfully scheduled")
                : (false, $"Job {job.Key} schedule failed");
        }

        public async Task<(bool success, string message)> UnscheduleJobAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) return (false, "Key was empty");

            if (_timers.Count == 0) return (true, "Scheduler was empty");

            if (!_timers.ContainsKey(key)) return (true, $"Job {key} does not scheduled");

            await _timers.First(pair => pair.Key == key && pair.Value.Item1 != null).Value.Item1.DisposeAsync();

            var removeResult = _timers.Remove(key);

            return removeResult
                ? (true, $"Job {key} was successfully unscheduled")
                : (false, $"Remove job {key} from scheduler was failed");
        }

        public async Task<(bool success, string message)> RescheduleJobAsync(Job job)
        {
            if (_timers.Count > 0 && _timers.ContainsKey(job.Key))
            {
                var removeResult = _timers.Remove(job.Key);
                if (!removeResult) return (false, $"Remove job {job.Key} from scheduler was failed");
            }

            var timer = CreateNewTimer(job);
            var addResult = _timers.TryAdd(job.Key, (timer, job));

            return addResult
                ? (true, $"Job {job.Key} was successfully scheduled")
                : (false, $"Job {job.Key} schedule failed");
        }

        public async Task<(bool success, string message, Job job)> GetJob(string key)
        {
            if (_timers.Count == 0) return (false, "Scheduler is empty", null);

            if (_timers.ContainsKey(key)) return (true, $"Job: {key} is active", null);

            return (false, $"Job: {key} is not scheduled", null);
        }

        public async Task<(bool success, string message, Job[] jobs)> GetJobs()
        {
            return _timers.Count == 0
                ? (false, "Scheduler is empty", null)
                : (true, "That's ur scheduled jobs, boy", _timers.Values.Select(x => x.Item2).ToArray());
        }

        private Timer CreateNewTimer(Job job) => new Timer(
            s => job.Task.Invoke(s),
            job.TaskParameters,
            job.Schedule.GetStartJobTimeSpan(),
            job.Schedule.GetPeriodJobTimeSpan());
    }
}