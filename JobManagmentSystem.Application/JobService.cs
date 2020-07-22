using System;
using System.Threading.Tasks;
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

        public JobService(IScheduler scheduler, TaskFactory factory, IPersistStorage storage,
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
                var task = _factory.Create(dto.TaskName);

                var job = new Job(task, Convert.ToDateTime(dto.TimeStart), dto.Interval, dto.IntervalType,
                    dto.TaskName, dto.TaskParameters, null);


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
                return await _scheduler.UnscheduleJobAsync(key);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }

        public async Task<Result> RescheduleJobAsync(JobDto dto)
        {
            try
            {
                var task = _factory.Create(dto.TaskName);

                var job = new Job(task, Convert.ToDateTime(dto.TimeStart), dto.Interval, dto.IntervalType,
                    dto.TaskName, dto.TaskParameters, dto.Key);

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

        public async Task<Result<Job>> GetJobByIdAsync(string key)
        {
            try
            {
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