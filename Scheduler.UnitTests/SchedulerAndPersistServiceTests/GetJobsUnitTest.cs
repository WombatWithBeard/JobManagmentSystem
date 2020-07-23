using System;
using System.IO;
using System.Threading.Tasks;
using JobManagmentSystem.FileStorage;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Scheduler.UnitTests.SchedulerAndPersistServiceTests
{
    public class GetJobsUnitTest : IDisposable
    {
        private readonly JobManagmentSystem.Scheduler.Scheduler _scheduler;
        private readonly TestJobMaker _jobMaker;
        private readonly IScheduler _persistentScheduler;
        private readonly IPersistStorage _storage;
        private string Path = $@"\{nameof(GetJobsUnitTest)}.ndjson";

        public GetJobsUnitTest()
        {
            _jobMaker = new TestJobMaker();
            _scheduler =
                new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);
            _storage = new JobsFileStorage(NullLogger<JobsFileStorage>.Instance, Path);
            _persistentScheduler = new PersistentScheduler(_scheduler, _storage,
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
            if (File.Exists(Directory.GetCurrentDirectory() + Path))
            {
                File.Delete(Directory.GetCurrentDirectory() + Path);
            }
        }
    }
}