using System;
using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.Scheduler.Common.Models;
using Microsoft.Extensions.Logging;

namespace JobManagmentSystem.Scheduler
{
    public class PersistScheduler : IScheduler
    {
        private readonly IScheduler _scheduler;
        private readonly IPersistStorage _storage;
        private readonly ILogger<PersistScheduler> _logger;

        public PersistScheduler(IScheduler scheduler, IPersistStorage storage,
            ILogger<PersistScheduler> logger)
        {
            _scheduler = scheduler;
            _storage = storage;
            _logger = logger;
        }

        public async Task<(bool success, string message)> ScheduleJob(Job job)
        {
            try
            {
                var addJob = await _scheduler.ScheduleJob(job);
                if (!addJob.success) return (addJob.success, addJob.message);

                var saveJob = await _storage.SaveJobAsync(JsonSerializer.Serialize(job), job.Key);
                if (!saveJob.success) return (saveJob.success, saveJob.message);

                var unscheduledJob = await _scheduler.UnscheduleJobById(job.Key);
                if (unscheduledJob.success) return (addJob.success, addJob.message);

                var counter = 0;
                while (counter <= 3 || unscheduledJob.success)
                {
                    unscheduledJob = await _scheduler.UnscheduleJobById(job.Key);
                    await Task.Delay(1500);
                    counter++;
                }

                return (unscheduledJob.success, unscheduledJob.message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        //TODO: what i need to do? add TryTo... with while?
        public async Task<(bool success, string message)> RescheduleJob(Job job)
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

                var unscheduledJob = await _scheduler.UnscheduleJobById(job.Key);
                if (!unscheduledJob.success) return (unscheduledJob.success, unscheduledJob.message);

                var saveJob = await _storage.SaveJobAsync(JsonSerializer.Serialize(job), job.Key);
                if (!saveJob.success) return (saveJob.success, saveJob.message);

                var addJob = await _scheduler.ScheduleJob(job);
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

        public async Task<(bool success, string message)> UnscheduleJobById(string key)
        {
            try
            {
                var deleteJob = await _storage.DeleteJobAsync(key);
                if (!deleteJob.success) return (deleteJob.success, deleteJob.message);

                //TODO: check for exist in schedule and return not exists if false in both?

                var unscheduledJob = await _scheduler.UnscheduleJobById(key);
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
        public async Task<(bool success, string message)> UnscheduleAllJobs()
        {
            try
            {
                var unscheduledJobs = await _scheduler.UnscheduleAllJobs();
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