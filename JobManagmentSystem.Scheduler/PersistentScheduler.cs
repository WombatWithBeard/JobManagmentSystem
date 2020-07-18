﻿using System;
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

                var unscheduledJob = await _scheduler.UnscheduleJobAsync(job.Key);
                if (unscheduledJob.success)
                {
                    _logger.LogError($"Job {job.Key} was successfully unscheduled while SaveJobException");
                    throw;
                }

                var counter = 0;
                while (counter <= 3 || unscheduledJob.success)
                {
                    await Task.Delay(1500);
                    unscheduledJob = await _scheduler.UnscheduleJobAsync(job.Key);
                    counter++;
                }

                _logger.LogError(unscheduledJob.success
                    ? $"Job {job.Key} was successfully unscheduled while {nameof(SaveJobException)}"
                    : $"Job {job.Key} unscheduled while SaveJobException was failed");

                throw;
                //TODO: exception while catch?
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
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

                _logger.LogError(e.Message);
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                throw;
            }
        }
        
        public async Task<(bool success, string message)> RescheduleJobAsync(Job job)
        {
            try
            {
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
    }
}