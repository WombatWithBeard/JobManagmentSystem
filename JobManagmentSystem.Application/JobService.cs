using System;
using System.Collections.Generic;
using System.Text.Json;
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
        private readonly ILogger<JobService> _logger;

        public JobService(IScheduler scheduler, TaskFactory factory,
            ILogger<JobService> logger)
        {
            _scheduler = scheduler;
            _factory = factory;
            _logger = logger;
        }

        public async Task<(bool success, string message)> ScheduleJobAsync(JobDto dto)
        {
            try
            {
                var task = _factory.Create(dto.TaskName, dto.TaskParameters);

                var job = new Job(task, Convert.ToDateTime(dto.TimeStart), dto.Interval, dto.IntervalType,
                    dto.TaskName, dto.TaskParameters);

                return await _scheduler.ScheduleJob(job);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<(bool success, string message)> DeleteJobAsync(string key)
        {
            try
            {
                return await _scheduler.DeleteJobAsync(key);
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
                var task = _factory.Create(dto.TaskName, dto.TaskParameters);

                var job = new Job(task, Convert.ToDateTime(dto.TimeStart), dto.Interval, dto.IntervalType,
                    dto.TaskName, dto.TaskParameters);

                return await _scheduler.ReScheduleJobAsync(job);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<Dictionary<string, (bool success, string message)>> ReScheduleAllJobsAsync()
        {
            try
            {
                var jobsListAsync = await _scheduler.GetJobsListAsync();

                if (!jobsListAsync.success) throw new Exception(jobsListAsync.message);

                if (jobsListAsync.success && jobsListAsync.jobs.Length <= 0) throw new Exception("No data available");

                //TODO: Need mapping
                var dict = new Dictionary<string, (bool success, string message)>();

                foreach (var jobS in jobsListAsync.jobs)
                {
                    var job = JsonSerializer.Deserialize<Job>(jobS);

                    job.Task = _factory.Create(job.Name, job.TaskParameters);

                    var reScheduleResult = await _scheduler.ReScheduleJobAsync(job);

                    dict.Add(job.Key, reScheduleResult);
                }

                return dict;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<(bool success, string message, Job job)> GetScheduledJobByIdAsync(string key)
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

        public async Task<(bool success, string message, List<Job> jobs)> GetAllSchedulerJobsAsync()
        {
            try
            {
                return await _scheduler.GetJobsListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }
        
        // public async Task<(bool success, string message, string job)> GetJobAsync(string key)
        // {
        //     try
        //     {
        //         return await _storage.GetJobAsync(key);
        //     }
        //     catch (Exception e)
        //     {
        //         _logger.LogError(e.Message);
        //         return (false, e.Message, null);
        //     }
        // }
        //
        // public async Task<(bool success, string message, string[] jobs)> GetJobsListAsync()
        // {
        //     try
        //     {
        //         return await _storage.GetJobsAsync();
        //     }
        //     catch (Exception e)
        //     {
        //         _logger.LogError(e.Message);
        //         return (false, e.Message, null);
        //     }
        // }
    }
}

