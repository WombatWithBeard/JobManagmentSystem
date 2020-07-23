using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Scheduler.UnitTests.SchedulerServiceTests
{
    public class RescheduleJobUnitTests
    {
        private readonly IScheduler _scheduler;
        private readonly TestJobMaker _jobMaker;

        public RescheduleJobUnitTests()
        {
            _jobMaker = new TestJobMaker();
            _scheduler =
                new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);
        }

        [Fact]
        public async Task RescheduleJob_ValidResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();
            var updateJob = _jobMaker.CreateTestJob(job.Key);

            //Act
            await _scheduler.ScheduleJobAsync(job);
            var result = await _scheduler.RescheduleJobAsync(updateJob);

            //Assert
            Assert.True(result.Success);
            // Assert.Equal($"Job {job.Key} was successfully scheduled", message);
        }

        [Fact]
        public async Task RescheduleJobWithEmptyScheduleAndPersistence_ValidResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            var result = await _scheduler.RescheduleJobAsync(job);

            //Assert
            Assert.True(result.Success);
        }
    }
}