using System;
using System.Threading.Tasks;
using JobManagmentSystem.Application.Common.Exceptions;
using JobManagmentSystem.Application.Common.Interfaces;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.Scheduler.Common.Results;
using JobManagmentSystem.Scheduler.Models;
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

        public async Task<Result> ScheduleJobAsync(JobDto dto)
        {
            try
            {
                var task = _factory.Create(dto.Name);

                var job = new Job(task, Convert.ToDateTime(dto.TimeStart), dto.Interval, dto.IntervalType,
                    dto.Name, dto.TaskParameters, dto.Key);


                return await _scheduler.ScheduleJobAsync(job);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<Result> UncheduleJobAsync(string key)
        {
            try
            {
                IsKeyEmptyOrNull(key);

                return await _scheduler.UnscheduleJobAsync(key);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        private void IsKeyEmptyOrNull(string key)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrWhiteSpace(key))
            {
                throw new WrongKeyBadRequestException(key);
            }
        }

        public async Task<Result> RescheduleJobAsync(JobDto dto)
        {
            try
            {
                IsKeyEmptyOrNull(dto.Key);
                var task = _factory.Create(dto.Name);

                var job = new Job(task, Convert.ToDateTime(dto.TimeStart), dto.Interval, dto.IntervalType,
                    dto.Name, dto.TaskParameters, dto.Key);

                return await _scheduler.RescheduleJobAsync(job);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<Result<Job>> GetJobByIdAsync(string key)
        {
            try
            {
                IsKeyEmptyOrNull(key);
                return await _scheduler.GetJob(key);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<Result<Job[]>> GetAllJobsAsync()
        {
            try
            {
                return await _scheduler.GetJobs();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }
    }
}