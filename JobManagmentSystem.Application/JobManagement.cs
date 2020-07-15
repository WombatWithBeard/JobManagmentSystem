using System;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler;
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

                var job = new Job(task, dto.TimeStart, (int) dto.Interval, dto.TaskName); //TODO: interval

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

                var job = new Job(task, dto.TimeStart, (int) dto.Interval, dto.TaskName); //TODO: interval

                return await _schedulerAndPersistence.ReScheduleJobAsync(job);
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