using System.Linq;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.Scheduler.Common.Results;
using JobManagmentSystem.Scheduler.Models;
using Microsoft.Extensions.Logging;

namespace JobManagmentSystem.Scheduler
{
    public class PersistentScheduler : IScheduler
    {
        private readonly Scheduler _scheduler;
        private readonly IPersistStorage _storage;
        private readonly ILogger<PersistentScheduler> _logger;


        public PersistentScheduler(Scheduler scheduler, IPersistStorage storage,
            ILogger<PersistentScheduler> logger)
        {
            _scheduler = scheduler;
            _storage = storage;
            _logger = logger;
        }

        public async Task<Result> ScheduleJobAsync(Job job)
        {
            var schedulingResult = await _scheduler.ScheduleJobAsync(job);
            var savingResult = await _storage.SaveJobAsync(job);

            //TODO: check on both 

            return Result.Combine(schedulingResult, savingResult)
                .OnFailure(async () => await UnscheduleJobAsync(job.Key))
                .OnBoth(result => _logger.LogInformation($"Job {job.Key} was scheduled and saved successfully"));
        }

        public async Task<Result> UnscheduleJobAsync(string key)
        {
            var unscheduledJob = await TryUnscheduleJob(key);
            var deletedJob = await TryDeleteJob(key);

            return Result.Combine(unscheduledJob, deletedJob);
        }

        private async Task<Result> TryUnscheduleJob(string key)
        {
            var unscheduledJob = await _scheduler.UnscheduleJobAsync(key);

            return unscheduledJob.OnFailure(async () =>
            {
                var counter = 0;
                while (counter <= 3 || unscheduledJob.Success)
                {
                    await Task.Delay(1500);
                    unscheduledJob = await _scheduler.UnscheduleJobAsync(key);
                    counter++;
                }
            });
        }

        private async Task<Result> TryDeleteJob(string key)
        {
            var deleteJob = await _storage.DeleteJobAsync(key);

            return deleteJob.OnFailure(async () =>
            {
                var counter = 0;
                while (counter <= 3 || deleteJob.Success)
                {
                    await Task.Delay(1500);
                    deleteJob = await _storage.DeleteJobAsync(key);
                    counter++;
                }
            });
        }

        public async Task<Result> RescheduleJobAsync(Job job)
        {
            var unscheduleJobResult = await UnscheduleJobAsync(job.Key);
            var schedulerJobResult = await ScheduleJobAsync(job);

            return Result.Combine(unscheduleJobResult, schedulerJobResult);
        }

        public async Task<Result<Job>> GetJob(string key)
        {
            var scheduledJobResult = await _scheduler.GetJob(key);
            var savedJobResult = await _storage.GetJobAsync(key);

            if (!scheduledJobResult.Success && savedJobResult.Success) return savedJobResult;
            if (scheduledJobResult.Success && !savedJobResult.Success) return scheduledJobResult;

            return Result.Combine(scheduledJobResult, savedJobResult)
                .OnSuccess(() => AggregatedJob(scheduledJobResult.Value, savedJobResult.Value));
        }

        private Job AggregatedJob(Job scheduledJobValue, Job savedJobValue)
        {
            //TODO: and this shit
            if (scheduledJobValue.Key != savedJobValue.Key) return scheduledJobValue;

            scheduledJobValue.Scheduled = true;
            scheduledJobValue.Persisted = true;

            return scheduledJobValue;
        }

        public async Task<Result<Job[]>> GetJobs()
        {
            var scheduledJobsResult = await _scheduler.GetJobs();
            var savedJobsResult = await _storage.GetJobsAsync();

            if (!scheduledJobsResult.Success && savedJobsResult.Success) return savedJobsResult;
            if (scheduledJobsResult.Success && !savedJobsResult.Success) return scheduledJobsResult;

            return Result.Combine(scheduledJobsResult, savedJobsResult).OnSuccess(() =>
                AggregatedJobs(scheduledJobsResult.Value, savedJobsResult.Value));
        }

        //TODO: beautify this
        private Job[] AggregatedJobs(Job[] scheduledJobs, Job[] persistedJobs)
        {
            if (scheduledJobs == null && persistedJobs != null) return ProcessedJobs(persistedJobs, 1);
            if (persistedJobs == null) return ProcessedJobs(scheduledJobs, 0);

            var runningJobsDict = scheduledJobs.ToDictionary(job => job.Key);
            var persistedJobsDict = persistedJobs.ToDictionary(job => job.Key);

            var unionJobs = runningJobsDict
                .Union(persistedJobsDict)
                .GroupBy(g => g.Key)
                .ToDictionary(k => k.Key, g => g.First().Value);

            foreach (var (key, value) in unionJobs)
            {
                if (runningJobsDict.ContainsKey(key) && !persistedJobsDict.ContainsKey(key))
                {
                    value.Persisted = false;
                    value.Scheduled = true;
                }
                else if (!runningJobsDict.ContainsKey(key) && persistedJobsDict.ContainsKey(key))
                {
                    value.Persisted = true;
                    value.Scheduled = false;
                }
                else
                {
                    value.Persisted = true;
                    value.Scheduled = true;
                }
            }

            return unionJobs.Values.ToArray();
        }

        private Job[] ProcessedJobs(Job[] jobs, int type)
        {
            var result = jobs.ToList();

            //TODO: TYPE ?????? Enum?
            if (type == 1)
            {
                result.ForEach(j => j.Persisted = true);
                result.ForEach(j => j.Scheduled = false);
            }
            else
            {
                result.ForEach(j => j.Persisted = false);
                result.ForEach(j => j.Scheduled = true);
            }

            return result.ToArray();
        }
    }
}