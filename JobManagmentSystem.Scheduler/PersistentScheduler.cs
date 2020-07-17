using System;
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
                var addJob = await _scheduler.ScheduleJobAsync(job);
                if (!addJob.success) return (addJob.success, addJob.message);

                var saveJob = await _storage.SaveJobAsync(JsonSerializer.Serialize(job), job.Key);
                if (saveJob.success) return (addJob.success, addJob.message);

                var unscheduledJob = await _scheduler.UnscheduleJobByIdAsync(job.Key);
                if (unscheduledJob.success) return (saveJob.success, saveJob.message);

                var counter = 0;
                while (counter <= 3 || unscheduledJob.success)
                {
                    await Task.Delay(1500);
                    unscheduledJob = await _scheduler.UnscheduleJobByIdAsync(job.Key);
                    counter++;
                }

                return (saveJob.success, saveJob.message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        //TODO: what i need to do? add TryTo... with while?
        public async Task<(bool success, string message)> RescheduleJobAsync(Job job)
        {
            try
            {
                var deleteJob = await _storage.DeleteJobAsync(job.Key);
                if (!deleteJob.success)
                {
                    if (deleteJob.message != "Key not exists" || deleteJob.message != "File not exists")
                    {
                        return (deleteJob.success, deleteJob.message);
                    }
                }

                var unscheduledJob = await _scheduler.UnscheduleJobByIdAsync(job.Key);
                if (!unscheduledJob.success) return (unscheduledJob.success, unscheduledJob.message);

                var saveJob = await _storage.SaveJobAsync(JsonSerializer.Serialize(job), job.Key);
                if (!saveJob.success) return (saveJob.success, saveJob.message);

                var addJob = await _scheduler.ScheduleJobAsync(job);
                if (addJob.success) return (addJob.success, addJob.message);

                //TODO: Again

                return (addJob.success, addJob.message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        public async Task<(bool success, string message)> UnscheduleJobByIdAsync(string key)
        {
            try
            {
                var deleteJob = await _storage.DeleteJobAsync(key);
                if (!deleteJob.success) return (deleteJob.success, deleteJob.message);

                //TODO: check for exist in schedule and return not exists if false in both?

                var unscheduledJob = await _scheduler.UnscheduleJobByIdAsync(key);
                if (unscheduledJob.success) return (unscheduledJob.success, unscheduledJob.message);

                return (unscheduledJob.success, "Error while try to unschedule job");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        //TODO: do i really need unschedule all jobs?
        public async Task<(bool success, string message)> UnscheduleAllJobsAsync()
        {
            try
            {
                var unscheduledJobs = await _scheduler.UnscheduleAllJobsAsync();
                if (!unscheduledJobs.success) return (unscheduledJobs.success, unscheduledJobs.message);

                var deletedJobs = await _storage.DeleteAllJobAsync();
                if (!deletedJobs.success) return (deletedJobs.success, deletedJobs.message);

                return (unscheduledJobs.success, unscheduledJobs.message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }
    }
}