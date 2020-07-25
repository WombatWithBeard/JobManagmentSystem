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
    public class UnscheduleJobPsUnitTests : IDisposable
    {
        private readonly TestJobMaker _jobMaker;
        private readonly IScheduler _persistentScheduler;
        private readonly string _path = $@"\{nameof(UnscheduleJobPsUnitTests)}.ndjson";

        public UnscheduleJobPsUnitTests()
        {
            _jobMaker = new TestJobMaker();
            var scheduler = new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);
            var options = Options.Create(new FileStorage
                {StoragePath = $"{nameof(UnscheduleJobPsUnitTests)}.ndjson"});
            IPersistStorage storage = new JobsFileStorage(NullLogger<JobsFileStorage>.Instance, options);
            _persistentScheduler = new PersistentScheduler(scheduler, storage,
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
        }

        [Fact]
        public async Task DeleteJob_NotFoundResult()
        {
            //Assert
            await Assert.ThrowsAsync<NotFoundException>(() => _persistentScheduler.UnscheduleJobAsync("test789"));
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