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
            if (_timers.ContainsKey(job.Key)) return Result.Fail($"Job {job.Key} already scheduled");

            var timer = CreateNewTimer(job);
            
            var addResult = _timers.TryAdd(job.Key, (timer, job));

            return addResult
                ? Result.Ok().OnSuccess(() => _logger.LogInformation($"Job {job.Key} was successfully scheduled"))
                : Result.Fail($"Job {job.Key} schedule failed");
        }

        public async Task<Result> UnscheduleJobAsync(string key)
        {
            if (string.IsNullOrEmpty(key)) return Result.Fail("Key was empty");

            if (_timers.Count == 0) return Result.Ok();

            if (!_timers.ContainsKey(key)) return Result.Ok();

            await _timers.First(pair => pair.Key == key && pair.Value.Item1 != null).Value.Item1.DisposeAsync();

            var removeResult = _timers.Remove(key);

            return removeResult
                ? Result.Ok().OnSuccess(() => _logger.LogInformation($"Job {key} was successfully unscheduled"))
                : Result.Fail($"Remove job {key} from scheduler was failed");
        }

        public async Task<Result> RescheduleJobAsync(Job job)
        {
            if (_timers.Count > 0 && _timers.ContainsKey(job.Key))
            {
                var removeResult = await UnscheduleJobAsync(job.Key); //_timers.Remove(job.Key);
                if (!removeResult.Success) return Result.Fail($"Remove job {job.Key} from scheduler was failed");
            }

            var timer = CreateNewTimer(job);
            var addResult = _timers.TryAdd(job.Key, (timer, job));

            return addResult
                ? Result.Ok()
                : Result.Fail($"Job {job.Key} schedule failed");
        }

        public async Task<Result<Job>> GetJob(string key)
        {
            if (_timers.Count == 0) return Result.Fail<Job>("Scheduler is empty");

            if (!_timers.ContainsKey(key)) return Result.Fail<Job>($"Job: {key} is not scheduled");

            var job = _timers.FirstOrDefault(j => j.Key == key).Value.Item2;

            return Result.Ok(job);
        }

        public async Task<Result<Job[]>> GetJobs()
        {
            return _timers.Count == 0
                ? Result.Fail<Job[]>("Scheduler is empty") //new Result<Job[]>(null, false, "Scheduler is empty")
                : Result.Ok(_timers.Values.Select(x => x.Item2).ToArray());
            //new Result<Job[]>(_timers.Values.Select(x => x.Item2).ToArray(), true,
            // "That's ur scheduled jobs, boy");
        }

        private Timer CreateNewTimer(Job job) => new Timer(
            s => job.Task.Invoke(s),
            job.TaskParameters,
            job.Schedule.GetStartJobTimeSpan(),
            job.Schedule.GetPeriodJobTimeSpan());
    }
}