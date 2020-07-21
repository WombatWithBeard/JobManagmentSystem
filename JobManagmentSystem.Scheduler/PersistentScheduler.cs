using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.Scheduler.Common.Models;
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

        public async Task<(bool success, string message)> ScheduleJobAsync(Job job)
        {
            try
            {
                if (job.Status)
                {
                    var addJob = await _scheduler.ScheduleJobAsync(job);
                    if (!addJob.success) throw new ScheduleJobException(job.Name, job.Key);
                }

                var saveJob = await _storage.SaveJobAsync(JsonSerializer.Serialize(job), job.Key);
                if (!saveJob.success) throw new SaveJobException(job.Name, job.Key);

                return (saveJob.success, saveJob.message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                await UnscheduleJobAsync(job.Key);
                throw;
            }
        }

        public async Task<(bool success, string message)> UnscheduleJobAsync(string key)
        {
            try
            {
                var unscheduledJob = await TryUnscheduleJob(key);
                var deleteJob = await TryDeleteJob(key);

                if (!unscheduledJob.success) throw new UnscheduleJobException(key);
                if (!deleteJob.success) throw new DeleteJobException(key);

                return (unscheduledJob.success, unscheduledJob.message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        private async Task<(bool success, string message)> TryUnscheduleJob(string key)
        {
            try
            {
                var unscheduledJob = await _scheduler.UnscheduleJobAsync(key);
                if (unscheduledJob.success)
                {
                    _logger.LogInformation(unscheduledJob.message);
                    return (true, unscheduledJob.message);
                }

                //TODO: what about - not exists? need to change this

                var counter = 0;
                while (counter <= 3 || unscheduledJob.success)
                {
                    await Task.Delay(1500);
                    unscheduledJob = await _scheduler.UnscheduleJobAsync(key);
                    counter++;
                }

                _logger.LogInformation(unscheduledJob.message);

                return (unscheduledJob.success, unscheduledJob.message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        private async Task<(bool success, string message)> TryDeleteJob(string key)
        {
            try
            {
                var deleteJob = await _storage.DeleteJobAsync(key);
                if (deleteJob.success)
                {
                    _logger.LogInformation(deleteJob.message);
                    return (deleteJob.success, deleteJob.message);
                }

                //TODO: what about - not exists? need to change this

                var counter = 0;
                while (counter <= 3 || deleteJob.success)
                {
                    await Task.Delay(1500);
                    deleteJob = await _storage.DeleteJobAsync(key);
                    counter++;
                }

                _logger.LogError(deleteJob.message);

                return (deleteJob.success, deleteJob.message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<(bool success, string message)> RescheduleJobAsync(Job job)
        {
            try
            {
                await UnscheduleJobAsync(job.Key);

                return await ScheduleJobAsync(job);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        public async Task<(bool success, string message, Job job)> GetJob(string key)
        {
            try
            {
                var scheduledJob = await _scheduler.GetJob(key);

                var savedJob = await _storage.GetJobAsync(key);

                if (scheduledJob.success && !savedJob.success)
                    return (false, $"Job {key} scheduled, by has no saved data", null);

                if (!scheduledJob.success && savedJob.success)
                    return (false, $"Job {key} not scheduled, by has saved data", Job.Empty);

                if (!savedJob.success && !scheduledJob.success) throw new NotFoundException(key);

                return (savedJob.success, savedJob.message, Job.Empty);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<(bool success, string message, Job[] jobs)> GetJobs()
        {
            try
            {
                (bool schedulerOk, string message, Job[] runningJobs) = await _scheduler.GetJobs();
                (bool storageOk, string msg, string[] persistedJobs) = await _storage.GetJobsAsync();

                if (!schedulerOk && !storageOk) return (false, "No jobs was found", null);

                if (schedulerOk && !storageOk)
                    return (true, "Job id only from scheduler", runningJobs);

                var persistedJobsS = MapToJobs(persistedJobs);
                if (!schedulerOk) return (true, "Jobs from storage", persistedJobsS);

                return (true, "Jobs from storage and scheduler", AggregatedJobs(runningJobs, persistedJobsS));
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        private Job[] AggregatedJobs(Job[] runningJobs, Job[] persistedJobsS)
        {
            var result = new List<Job>();

            foreach (var runningJob in runningJobs)
            {
                //Test
                var a = persistedJobsS.Contains(runningJob);

                if (persistedJobsS.Any(j => j.Key == runningJob.Key))
                {
                    runningJob.Status = Job.JobStatusConsts.PersistAndScheduled;
                    result.Add(runningJob);
                }
                else
                {
                    runningJob.Status = Job.JobStatusConsts.ScheduledNotPersist;
                    result.Add(runningJob);
                }
            }

            foreach (var job in persistedJobsS)
            {
                if (runningJobs.Contains(job)) continue;
                job.Status = Job.JobStatusConsts.PersistNotScheduled;
                result.Add(job);
            }

            return result.ToArray();
        }

        private Job[] MapToJobs(string[] persistedJobs) =>
            persistedJobs.Select(j => JsonSerializer.Deserialize<Job>(j)).ToArray();
    }
}