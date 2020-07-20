using System;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.Scheduler.Common.Models;
using Microsoft.Extensions.Logging;

namespace JobManagmentSystem.Application
{
    public class JobService : IJobService
    {
        private readonly IScheduler _scheduler;
        private readonly TaskFactory _factory;
        private readonly IPersistStorage _storage;
        private readonly ILogger<JobService> _logger;

        public JobService(IScheduler scheduler, TaskFactory factory, IPersistStorage storage,
            ILogger<JobService> logger)
        {
            _scheduler = scheduler;
            _factory = factory;
            _storage = storage;
            _logger = logger;
        }

        public async Task<(bool success, string message)> ScheduleJobAsync(JobDto dto)
        {
            try
            {
                var task = _factory.Create(dto.TaskName);

                var job = new Job(task, Convert.ToDateTime(dto.TimeStart), dto.Interval, dto.IntervalType,
                    dto.TaskName, dto.TaskParameters, null, dto.Enabled);

                return await _scheduler.ScheduleJobAsync(job);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<(bool success, string message)> UncheduleJobAsync(string key)
        {
            try
            {
                return await _scheduler.UnscheduleJobAsync(key);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<(bool success, string message)> RescheduleJobAsync(JobDto dto)
        {
            try
            {
                var task = _factory.Create(dto.TaskName);

                var job = new Job(task, Convert.ToDateTime(dto.TimeStart), dto.Interval, dto.IntervalType,
                    dto.TaskName, dto.TaskParameters, dto.Key, dto.Enabled);

                return await _scheduler.RescheduleJobAsync(job);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        // public async Task<Dictionary<string, (bool success, string message)>> ReScheduleAllJobsAsync()
        // {
        //     try
        //     {
        //         var jobsListAsync = await _scheduler.GetJobsListAsync();
        //
        //         if (!jobsListAsync.success) throw new Exception(jobsListAsync.message);
        //
        //         if (jobsListAsync.success && jobsListAsync.jobs.Length <= 0) throw new Exception("No data available");
        //
        //         //TODO: Need mapping
        //         var dict = new Dictionary<string, (bool success, string message)>();
        //
        //         foreach (var jobS in jobsListAsync.jobs)
        //         {
        //             var job = JsonSerializer.Deserialize<Job>(jobS);
        //
        //             job.Task = _factory.Create(job.Name, job.TaskParameters);
        //
        //             var reScheduleResult = await _scheduler.ReScheduleJobAsync(job);
        //
        //             dict.Add(job.Key, reScheduleResult);
        //         }
        //
        //         return dict;
        //     }
        //     catch (Exception e)
        //     {
        //         _logger.LogError(e.Message);
        //         throw;
        //     }
        // }

        public async Task<(bool success, string message, string job)> GetJobByIdAsync(string key)
        {
            try
            {
                return await _scheduler.GetJobAsync(key);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<(bool success, string message, string[] jobs)> GetAllJobsAsync()
        {
            try
            {
                return await _scheduler.GetJobsArrayAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }
    }
}