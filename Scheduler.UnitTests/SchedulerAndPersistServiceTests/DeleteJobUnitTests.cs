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
        private readonly IScheduler _scheduler;
        private readonly TestJobMaker _jobMaker;
        private readonly ISchedulerAndPersistence _schedulerAndPersistService;
        private readonly IPersistStorage _storage;

        public DeleteJobUnitTests()
        {
            _jobMaker = new TestJobMaker();
            _scheduler = new SchedulerService(NullLogger<SchedulerService>.Instance);
            _storage = new JobsFileStorage();
            _schedulerAndPersistService = new SchedulerAndPersistService(_scheduler, _storage,
                NullLogger<SchedulerAndPersistService>.Instance);
        }

        [Fact]
        public async Task DeleteJob_ValidResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            await _schedulerAndPersistService.CreateJobAsync(job);
            var (success, message) = await _schedulerAndPersistService.DeleteJobAsync(job.Key);

            //Assert
            Assert.True(success);
            Assert.Equal($"Job {job.Key} was successfully unscheduled", message);
        }

        [Fact]
        public async Task DeleteJob_KeyNotExistsInStorageResult()
        {
            //Act
            var (success, message) = await _schedulerAndPersistService.DeleteJobAsync("test");

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
            await _storage.SaveJobAsync(job);
            var (success, message) = await _schedulerAndPersistService.DeleteJobAsync(job.Key);

            //Assert
            Assert.True(success);
            Assert.Equal("Scheduler is empty", message);
        }

        [Fact]
        public async Task DeleteJob_JobNotExistsInSchedulerResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            _scheduler.AddJob(_jobMaker.CreateTestJob());
            await _storage.SaveJobAsync(job);
            var (success, message) = await _schedulerAndPersistService.DeleteJobAsync(job.Key);

            //Assert
            Assert.True(success);
            Assert.Equal("Job does not exist", message);
        }
    }
}