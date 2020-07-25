using System;
using System.IO;
using System.Threading.Tasks;
using JobManagmentSystem.FileStorage;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Exceptions;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Xunit;

namespace Scheduler.UnitTests.SchedulerAndPersistServiceTests
{
    public class RescheduleJobPsUnitTests : IDisposable
    {
        private readonly TestJobMaker _jobMaker;
        private readonly IScheduler _persistentScheduler;
        private readonly string _path = $@"\{nameof(RescheduleJobPsUnitTests)}.ndjson";

        public RescheduleJobPsUnitTests()
        {
            _jobMaker = new TestJobMaker();
            var scheduler = new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);
            var options = Options.Create(new FileStorage
                {StoragePath = $"{nameof(RescheduleJobPsUnitTests)}.ndjson"});
            IPersistStorage storage = new JobsFileStorage(NullLogger<JobsFileStorage>.Instance, options);
            _persistentScheduler = new PersistentScheduler(scheduler, storage,
                NullLogger<PersistentScheduler>.Instance);
        }

        [Fact]
        public async Task RescheduleJob_ValidResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();
            var updateJob = _jobMaker.CreateTestJob(job.Key);

            //Act
            await _persistentScheduler.ScheduleJobAsync(job);
            var result = await _persistentScheduler.RescheduleJobAsync(updateJob);

            //Assert
            Assert.True(result.Success);
            // Assert.Equal($"Job {job.Key} was successfully scheduled", message);
        }

        [Fact]
        public async Task RescheduleJob_NotFoundResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();
            
            //Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _persistentScheduler.RescheduleJobAsync(job));
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