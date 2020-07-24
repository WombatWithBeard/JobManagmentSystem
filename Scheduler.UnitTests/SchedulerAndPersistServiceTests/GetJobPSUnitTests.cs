﻿using System;
using System.Collections.Generic;
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
    public class GetJobPsUnitTests : IDisposable
    {
        private readonly JobManagmentSystem.Scheduler.Scheduler _scheduler;
        private readonly TestJobMaker _jobMaker;
        private readonly IScheduler _persistentScheduler;
        private readonly IPersistStorage _storage;
        private readonly string _path = $@"\{nameof(GetJobPsUnitTests)}.ndjson";

        public GetJobPsUnitTests()
        {
            _jobMaker = new TestJobMaker();
            _scheduler =
                new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);

            var options = Options.Create(new FileStorage
                {StoragePath = $"{nameof(GetJobPsUnitTests)}.ndjson"});

            _storage = new JobsFileStorage(NullLogger<JobsFileStorage>.Instance, options);
            _persistentScheduler = new PersistentScheduler(_scheduler, _storage,
                NullLogger<PersistentScheduler>.Instance);
        }

        [Fact]
        public async Task GetJobById_ValidResult()
        {
            //Arrange
            var newJob = _jobMaker.CreateTestJob();

            //Act
            await _persistentScheduler.ScheduleJobAsync(newJob);
            var result = await _persistentScheduler.GetJob(newJob.Key);

            //Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            // Assert.Equal($"Job: {newJob.Key} is active", message);
        }

        [Fact]
        public async Task GetJobById_SchedulerIsEmptyResult()
        {
            //Act
            var result = await _persistentScheduler.GetJob("Test");

            //Assert
            Assert.True(result.Failure);
            Assert.Equal("Scheduler is empty", result.Error);
        }

        [Fact]
        public async Task GetJobById_JobNotScheduledResult()
        {
            //Arrange
            var newJob = _jobMaker.CreateTestJob();

            //Act
            await _persistentScheduler.ScheduleJobAsync(newJob);
            var result = await _persistentScheduler.GetJob("Test");

            //Assert
            Assert.True(result.Failure);
            Assert.Equal("Job: Test is not scheduled", result.Error);
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