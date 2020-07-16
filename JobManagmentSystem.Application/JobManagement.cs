using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace JobManagmentSystem.Application
{
    public class JobManagement
    {
        private readonly ISchedulerAndPersistence _schedulerAndPersistence;
        private readonly TaskFactory _factory;
        private readonly ILogger<JobManagement> _logger;

        public JobManagement(ISchedulerAndPersistence schedulerAndPersistence, TaskFactory factory,
            ILogger<JobManagement> logger)
        {
            _schedulerAndPersistence = schedulerAndPersistence;
            _factory = factory;
            _logger = logger;
        }

        public async Task<(bool success, string message)> CreateJobAsync(JobDto dto)
        {
            try
            {
                var task = _factory.Create(dto.TaskName, dto.TaskParameters);

                var job = new Job(task, Convert.ToDateTime(dto.TimeStart), dto.Interval, dto.IntervalType,
                    dto.TaskName, dto.TaskParameters);

                return await _schedulerAndPersistence.CreateJobAsync(job);
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
                return await _schedulerAndPersistence.DeleteJobAsync(key);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<(bool success, string message)> ReScheduleJobAsync(JobDto dto)
        {
            try
            {
                var task = _factory.Create(dto.TaskName, dto.TaskParameters);

                var job = new Job(task, Convert.ToDateTime(dto.TimeStart), dto.Interval, dto.IntervalType,
                    dto.TaskName, dto.TaskParameters);

                return await _schedulerAndPersistence.ReScheduleJobAsync(job);
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
                var jobsListAsync = await _schedulerAndPersistence.GetJobsListAsync();

                if (!jobsListAsync.success) throw new Exception(jobsListAsync.message);

                if (jobsListAsync.success && jobsListAsync.jobs.Length <= 0) throw new Exception("No data available");

                //TODO: Need mapping
                var dict = new Dictionary<string, (bool success, string message)>();

                foreach (var jobS in jobsListAsync.jobs)
                {
                    var job = JsonSerializer.Deserialize<Job>(jobS);

                    job.Task = _factory.Create(job.Name, job.TaskParameters);

                    var reScheduleResult = await _schedulerAndPersistence.ReScheduleJobAsync(job);

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

        public async Task<(bool success, string message, string job)> GetJobAsync(string key)
        {
            try
            {
                return await _schedulerAndPersistence.GetJobAsync(key);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<(bool success, string message, string[] jobs)> GetJobsListAsync()
        {
            try
            {
                return await _schedulerAndPersistence.GetJobsListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }
    }
}