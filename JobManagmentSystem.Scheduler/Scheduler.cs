using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.Scheduler.Common.Results;
using JobManagmentSystem.Scheduler.Models;
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

        public async Task<Result> ScheduleJobAsync(Job job)
        {
            if (_timers.ContainsKey(job.Key)) return Result.Fail(SchedulerConsts.JobAlreadyScheduled);

            var timer = CreateNewTimer(job);

            var addResult = _timers.TryAdd(job.Key, (timer, job));

            return addResult
                ? Result.Ok().OnSuccess(() => _logger.LogInformation($"Job {job.Key} was successfully scheduled"))
                : Result.Fail(SchedulerConsts.JobScheduleFailed);
        }

        public async Task<Result> UnscheduleJobAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) return Result.Fail(SchedulerConsts.KeyWasEmpty);

            if (_timers.Count == 0) return Result.Fail(SchedulerConsts.SchedulerIsEmpty);

            if (!_timers.ContainsKey(key)) return Result.Fail(SchedulerConsts.KeyNotExists);

            await _timers.First(pair => pair.Key == key && pair.Value.Item1 != null).Value.Item1.DisposeAsync();

            var removeResult = _timers.Remove(key);

            return removeResult
                ? Result.Ok().OnSuccess(() => _logger.LogInformation($"Job {key} was successfully unscheduled"))
                : Result.Fail(SchedulerConsts.JobUnscheduleFailed);
        }

        public async Task<Result> RescheduleJobAsync(Job job)
        {
            if (_timers.Count > 0 && _timers.ContainsKey(job.Key))
            {
                var removeResult = await UnscheduleJobAsync(job.Key);
                if (!removeResult.Success) return Result.Fail(SchedulerConsts.RemoveJobFromSchedulerWasFailed);
            }

            var timer = CreateNewTimer(job);
            var addResult = _timers.TryAdd(job.Key, (timer, job));

            return addResult
                ? Result.Ok()
                : Result.Fail(SchedulerConsts.JobScheduleFailed);
        }

        public async Task<Result<Job>> GetJob(string key)
        {
            if (_timers.Count == 0) return Result.Fail<Job>(SchedulerConsts.SchedulerIsEmpty);

            if (!_timers.ContainsKey(key)) return Result.Fail<Job>(SchedulerConsts.JobIsNotScheduled);

            var job = _timers.FirstOrDefault(j => j.Key == key).Value.Item2;

            return Result.Ok(job);
        }

        public async Task<Result<Job[]>> GetJobs()
        {
            return _timers.Count == 0
                ? Result.Fail<Job[]>(SchedulerConsts.SchedulerIsEmpty)
                : Result.Ok(_timers.Values.Select(x => x.Item2).ToArray());
        }

        private Timer CreateNewTimer(Job job) => new Timer(
            s => job.Task.Invoke(s),
            job.TaskParameters,
            job.Schedule.GetStartJobTimeSpan(),
            job.Schedule.GetPeriodJobTimeSpan());
    }

    public class SchedulerConsts
    {
        public const string SchedulerIsEmpty = "Scheduler is empty";
        public const string KeyWasEmpty = "Key was empty";
        public const string KeyNotExists = "Key not exists";
        public const string JobIsNotScheduled = "Job is not scheduled";
        public const string JobScheduleFailed = "Job schedule failed";
        public const string JobUnscheduleFailed = "Job unschedule failed";
        public const string RemoveJobFromSchedulerWasFailed = "Remove job from scheduler was failed";
        public const string JobAlreadyScheduled = "Job already scheduled";
    }
}