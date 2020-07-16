using System;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobManagmentSystem.Scheduler
{
    public class SchedulerAndPersistService : ISchedulerAndPersistence
    {
        private readonly IScheduler _scheduler;
        private readonly IPersistStorage _storage;
        private readonly ILogger<SchedulerAndPersistService> _logger;

        public SchedulerAndPersistService(IScheduler scheduler, IPersistStorage storage,
            ILogger<SchedulerAndPersistService> logger)
        {
            _scheduler = scheduler;
            _storage = storage;
            _logger = logger;
        }

        public async Task<(bool success, string message)> CreateJobAsync(Job job)
        {
            try
            {
                var saveJob = await _storage.SaveJobAsync(job);
                if (!saveJob.success) return (saveJob.success, saveJob.message);

                var addJob = _scheduler.AddJob(job);
                if (addJob.success) return (addJob.success, addJob.message);

                var deleteJob = await _storage.DeleteJobAsync(job.Key);
                if (!deleteJob.success)
                {
                    //TODO: what need to do here?
                }

                return (addJob.success, addJob.message);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        public async Task<(bool success, string message)> DeleteJobAsync(string key)
        {
            try
            {
                var deleteJob = await _storage.DeleteJobAsync(key);
                if (!deleteJob.success) return (deleteJob.success, deleteJob.message);

                //TODO: check for exist in schedule and return not exists if false in both?

                var unscheduledJob = _scheduler.DeleteJobById(key);
                if (unscheduledJob.success) return (unscheduledJob.success, unscheduledJob.message);

                return (unscheduledJob.success, "Error while try to unschedule job");
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message);
            }
        }

        public async Task<(bool success, string message)> ReScheduleJobAsync(Job job)
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

                var unscheduledJob = _scheduler.DeleteJobById(job.Key);
                if (!unscheduledJob.success) return (unscheduledJob.success, unscheduledJob.message);

                var saveJob = await _storage.SaveJobAsync(job);
                if (!saveJob.success) return (saveJob.success, saveJob.message);

                var addJob = _scheduler.AddJob(job);
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

        public async Task<(bool success, string message, string job)> GetJobAsync(string key)
        {
            try
            {
                return await _storage.GetJobAsync(key);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message, null);
            }
        }

        public async Task<(bool success, string message, string[] jobs)> GetJobsListAsync()
        {
            try
            {
                return await _storage.GetJobsAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return (false, e.Message, null);
            }
        }
    }
}