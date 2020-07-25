using System.Threading.Tasks;
using JobManagmentSystem.Scheduler;
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
            var result = await _scheduler.GetJob(newJob.Key);

            //Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Value);
            // Assert.Equal($"Job: {newJob.Key} is active", message);
        }

        [Fact]
        public async Task GetJobById_SchedulerIsEmptyResult()
        {
            //Act
            var result = await _scheduler.GetJob("Test");

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
            await _scheduler.ScheduleJobAsync(newJob);
            var result = await _scheduler.GetJob("Test");

            //Assert
            Assert.True(result.Failure);
            Assert.Equal(SchedulerConsts.JobIsNotScheduled, result.Error);
        }
    }
}