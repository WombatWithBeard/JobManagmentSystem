using System;
using System.Reflection.Metadata;
using System.Text.Json;
using System.Threading.Tasks;
using JobManagmentSystem.FileStorage;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Scheduler.UnitTests.SchedulerAndPersistServiceTests
{
    public class DeleteJobUnitTests
    {
        private readonly JobManagmentSystem.Scheduler.Scheduler _scheduler;
        private readonly TestJobMaker _jobMaker;
        private readonly IScheduler _persistentScheduler;
        private readonly IPersistStorage _storage;

        public DeleteJobUnitTests()
        {
            _jobMaker = new TestJobMaker();
            _scheduler =
                new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);
            _storage = new JobsFileStorage(NullLogger<JobsFileStorage>.Instance);
            _persistentScheduler = new PersistentScheduler(_scheduler, _storage,
                NullLogger<PersistentScheduler>.Instance);
        }

        [Fact]
        public async Task DeleteJob_ValidResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            var a= _persistentScheduler.ScheduleJobAsync(job).Result;
            var result = await _persistentScheduler.UnscheduleJobAsync(job.Key);

            //Assert
            Assert.True(result.Success);
            // Assert.Equal($"Job {job.Key} was successfully unscheduled", message);
        }

        [Fact]
        public async Task DeleteJob_KeyNotExistsInStorageResult()
        {
            //Act
            var result = await _persistentScheduler.UnscheduleJobAsync("test");

            //Assert
            Assert.True(result.Failure);
            Assert.Equal("Key test not exists", result.Error);
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
    }
}