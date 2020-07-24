﻿using System;
using System.IO;
using System.Threading.Tasks;
using JobManagmentSystem.FileStorage;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Scheduler.UnitTests.SchedulerAndPersistServiceTests
{
    public class UnscheduleJobPsUnitTests : IDisposable
    {
        private readonly JobManagmentSystem.Scheduler.Scheduler _scheduler;
        private readonly TestJobMaker _jobMaker;
        private readonly IScheduler _persistentScheduler;
        private readonly IPersistStorage _storage;
        private readonly string _path = $@"\{nameof(UnscheduleJobPsUnitTests)}.ndjson";

        public UnscheduleJobPsUnitTests()
        {
            _jobMaker = new TestJobMaker();
            _scheduler =
                new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);
            var options = Options.Create(new FileStorage
                {StoragePath = $"{nameof(UnscheduleJobPsUnitTests)}.ndjson"});
            _storage = new JobsFileStorage(NullLogger<JobsFileStorage>.Instance, options);
            _persistentScheduler = new PersistentScheduler(_scheduler, _storage,
                NullLogger<PersistentScheduler>.Instance);
        }

        [Fact]
        public async Task DeleteJob_ValidResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            await _persistentScheduler.ScheduleJobAsync(job);
            var result = await _persistentScheduler.UnscheduleJobAsync(job.Key);

            //Assert
            Assert.True(result.Success);
            // Assert.Equal($"Job {job.Key} was successfully unscheduled", message);
        }

        [Fact]
        public async Task DeleteJob_SchedulerIsEmptyResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            await _storage.SaveJobAsync(job);
            var result = await _persistentScheduler.UnscheduleJobAsync(job.Key);

            //Assert
            Assert.True(result.Success);
            // Assert.Equal("Scheduler is empty", result.Error);
        }

        public void Dispose()
        {
            if (File.Exists(Directory.GetCurrentDirectory() + _path))
            {
                File.Delete(Directory.GetCurrentDirectory() + _path);
            }
        }
    }
}