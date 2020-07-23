using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Scheduler.UnitTests.SchedulerServiceTests
{
    public class ScheduleJobUnitTests
    {
        private readonly IScheduler _scheduler;
        private readonly TestJobMaker _jobMaker;

        public ScheduleJobUnitTests()
        {
            _jobMaker = new TestJobMaker();
            _scheduler =
                new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);
        }

        [Fact]
        public async Task AddJob_ValidResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            var result = await _scheduler.ScheduleJobAsync(job);

            //Assert
            Assert.True(result.Success);
            // Assert.Equal($"Job {job.Key} was successfully scheduled", result.Error);
        }

        [Fact]
        public async Task AddJob_KeyAlreadyExistsResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            await _scheduler.ScheduleJobAsync(job);
            var result = await _scheduler.ScheduleJobAsync(job);

            //Assert
            Assert.True(result.Failure);
            Assert.Equal($"Job {job.Key} already scheduled", result.Error);
        }
    }
}