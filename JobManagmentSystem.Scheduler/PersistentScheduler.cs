using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Exception;
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
                var addJob = await _scheduler.ScheduleJobAsync(job);
                if (!addJob.success) throw new ScheduleJobException(job.Name, job.Key);

                var saveJob = await _storage.SaveJobAsync(JsonSerializer.Serialize(job), job.Key);
                if (!saveJob.success) throw new SaveJobException(job.Name, job.Key);

                return (addJob.success, addJob.message);
            }
            catch (SaveJobException e)
            {
                _logger.LogError(e.Message);

                TryUnscheduleJob(job.Key);

                throw;
                //TODO: exception while catch?
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        private async void TryUnscheduleJob(string jobKey)
        {
            var unscheduledJob = await _scheduler.UnscheduleJobAsync(jobKey);
            if (unscheduledJob.success)
            {
                _logger.LogError($"Job {jobKey} was successfully unscheduled while SaveJobException");
                return;
            }

            var counter = 0;
            while (counter <= 3 || unscheduledJob.success)
            {
                await Task.Delay(1500);
                unscheduledJob = await _scheduler.UnscheduleJobAsync(jobKey);
                counter++;
            }

            _logger.LogError(unscheduledJob.success
                ? $"Job {jobKey} was successfully unscheduled while {nameof(SaveJobException)}"
                : $"Job {jobKey} unscheduled while SaveJobException was failed");
        }

        public async Task<(bool success, string message)> UnscheduleJobAsync(string key)
        {
            try
            {
                var unscheduledJob = await _scheduler.UnscheduleJobAsync(key);
                if (unscheduledJob.success) throw new UnscheduleJobException(key);

                var deleteJob = await _storage.DeleteJobAsync(key);
                if (!deleteJob.success) throw new DeleteJobException(key);

                return (unscheduledJob.success, unscheduledJob.message);
            }
            catch (DeleteJobException e)
            {
                _logger.LogError(e.Message);

                var result = await TryDeleteJob(key);
                if (result.success)
                    return (result.success, result.message);

                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        private async Task<(bool success, string message)> TryDeleteJob(string key)
        {
            await Task.Delay(1500);

            var deleteJob = await _storage.DeleteJobAsync(key);
            if (deleteJob.success) return (deleteJob.success, deleteJob.message);

            var counter = 0;
            while (counter <= 3 || deleteJob.success)
            {
                await Task.Delay(1500);
                deleteJob = await _storage.DeleteJobAsync(key);
                counter++;
            }

            if (deleteJob.success)
            {
                _logger.LogError($"Job {key} was successfully deleted while {nameof(DeleteJobException)}");
                return (deleteJob.success, deleteJob.message);
            }

            return (false, null);
        }

        public async Task<(bool success, string message)> RescheduleJobAsync(Job job)
        {
            try
            {
                //TODO: 
                var unscheduledJob = await UnscheduleJobAsync(job.Key);

                var scheduledJob = await ScheduleJobAsync(job);

                return (scheduledJob.success, scheduledJob.message);
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
                var scheduledJob = await _scheduler.GetJobAsync(key);

                var savedJob = await _storage.GetJobAsync(key);

                if (scheduledJob.success && !savedJob.success)
                    return (false, $"Job {key} scheduled, by has no saved data", null);

                if (!scheduledJob.success && savedJob.success)
                    return (false, $"Job {key} not scheduled, by has saved data", savedJob.job);

                if (!savedJob.success && !scheduledJob.success) throw new NotFoundException(key);

                return (savedJob.success, savedJob.message, savedJob.job);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<(bool success, string message, string[] jobs)> GetJobsArrayAsync()
        {
            try
            {
                var schedulerJobs = await _scheduler.GetJobsArrayAsync();
                var persistentJobs = await _storage.GetJobsAsync();

                if (!schedulerJobs.success && !persistentJobs.success) return (false, "No jobs was found", null);

                if (schedulerJobs.success && !persistentJobs.success)
                    return (true, "Job id only from scheduler", schedulerJobs.jobs);

                if (!schedulerJobs.success && persistentJobs.success)
                    return (true, "Jobs from storage", persistentJobs.jobs);

                var jobs = persistentJobs.jobs.Union(schedulerJobs.jobs).ToArray();

                return (true, "Jobs from storage, key from scheduler", jobs);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }
    }
}