using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Scheduler.UnitTests.SchedulerServiceTests
{
    public class AddJobUnitTests
    {
        private readonly IScheduler _scheduler;
        private readonly TestJobMaker _jobMaker;

        public AddJobUnitTests()
        {
            _jobMaker = new TestJobMaker();
            _scheduler = new SchedulerService(NullLogger<SchedulerService>.Instance);
        }

        [Fact]
        public void AddJob_ValidResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            var (success, message) = _scheduler.AddJob(job);

            //Assert
            Assert.True(success);
            Assert.Equal($"Job {job.Key} was successfully scheduled", message);
        }

        [Fact]
        public void AddJob_KeyAlreadyExistsResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            _scheduler.AddJob(job);
            var (success, message) = _scheduler.AddJob(job);

            //Assert
            Assert.False(success);
            Assert.Equal($"Job {job.Key} already exists", message);
        }
    }
}