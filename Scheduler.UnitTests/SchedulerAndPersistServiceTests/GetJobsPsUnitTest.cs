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
    public class GetJobsPsUnitTest : IDisposable
    {
        private readonly TestJobMaker _jobMaker;
        private readonly IScheduler _persistentScheduler;
        private readonly string _path = $@"\{nameof(GetJobsPsUnitTest)}.ndjson";

        public GetJobsPsUnitTest()
        {
            _jobMaker = new TestJobMaker();
            var scheduler = new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);
            var options = Options.Create(new FileStorage
                {StoragePath = $"{nameof(GetJobsPsUnitTest)}.ndjson"});
            IPersistStorage storage = new JobsFileStorage(NullLogger<JobsFileStorage>.Instance, options);
            _persistentScheduler = new PersistentScheduler(scheduler, storage,
                NullLogger<PersistentScheduler>.Instance);
        }

        [Fact]
        public async Task GetJobs_ValidResult()
        {
            //Act
            await _persistentScheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            await _persistentScheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            await _persistentScheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            await _persistentScheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            await _persistentScheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            var result = await _persistentScheduler.GetJobs();

            //Assert
            Assert.True(result.Success);
            Assert.Equal(5, result.Value.Length);
        }

        [Fact]
        public async Task GetJobs_EmptyResult()
        {
            //Act
            var result = await _persistentScheduler.GetJobs();

            //Assert
            Assert.True(result.Failure);
            Assert.Equal("Scheduler is empty", result.Error);
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