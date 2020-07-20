using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Scheduler.UnitTests.SchedulerServiceTests
{
    public class GetJobUnitTests
    {
        private readonly IScheduler _scheduler;
        private readonly TestJobMaker _jobMaker;

        public GetJobUnitTests()
        {
            _jobMaker = new TestJobMaker();
            _scheduler =
                new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);
        }

        [Fact]
        public async Task GetJobById_ValidResult()
        {
            //Arrange
            var newJob = _jobMaker.CreateTestJob();

            //Act
            await _scheduler.ScheduleJobAsync(newJob);
            var (success, message, job) = await _scheduler.GetJobAsync(newJob.Key);

            //Assert
            Assert.True(success);
            Assert.Equal($"Job: {newJob.Key} is active", message);
        }

        [Fact]
        public async Task GetJobById_SchedulerIsEmptyResult()
        {
            //Act
            var (success, message, job) = await _scheduler.GetJobAsync("Test");

            //Assert
            Assert.False(success);
            Assert.Equal("Scheduler is empty", message);
        }

        [Fact]
        public async Task GetJobById_JobNotScheduledResult()
        {
            //Arrange
            var newJob = _jobMaker.CreateTestJob();

            //Act
            await _scheduler.ScheduleJobAsync(newJob);
            var (success, message, job) = await _scheduler.GetJobAsync("Test");

            //Assert
            Assert.False(success);
            Assert.Equal("Job: Test is not scheduled", message);
        }
    }
}