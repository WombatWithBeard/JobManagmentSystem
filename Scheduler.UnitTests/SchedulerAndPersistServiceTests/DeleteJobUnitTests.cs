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
            await _persistentScheduler.ScheduleJobAsync(job);
            var (success, message) = await _persistentScheduler.UnscheduleJobAsync(job.Key);

            //Assert
            Assert.True(success);
            Assert.Equal($"Job {job.Key} was successfully unscheduled", message);
        }

        [Fact]
        public async Task DeleteJob_KeyNotExistsInStorageResult()
        {
            //Act
            var (success, message) = await _persistentScheduler.UnscheduleJobAsync("test");

            //Assert
            Assert.False(success);
            Assert.Equal("Key test not exists", message);
        }

        [Fact]
        public async Task DeleteJob_SchedulerIsEmptyResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            await _storage.SaveJobAsync(JsonSerializer.Serialize(job), job.Key);
            var (success, message) = await _persistentScheduler.UnscheduleJobAsync(job.Key);

            //Assert
            Assert.True(success);
            Assert.Equal("Scheduler is empty", message);
        }
    }
}