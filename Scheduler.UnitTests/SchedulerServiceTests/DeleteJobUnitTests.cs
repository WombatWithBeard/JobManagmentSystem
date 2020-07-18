using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Scheduler.UnitTests.SchedulerServiceTests
{
    public class DeleteJobUnitTests
    {
        private readonly IScheduler _scheduler;
        private readonly TestJobMaker _jobMaker;

        public DeleteJobUnitTests()
        {
            _jobMaker = new TestJobMaker();
            _scheduler =
                new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);
        }

        [Fact]
        public async Task DeleteJob_ValidResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            await _scheduler.ScheduleJobAsync(job);
            var (success, message) = await _scheduler.UnscheduleJobAsync(job.Key);

            //Assert
            Assert.True(success);
            Assert.Equal($"Job {job.Key} was successfully unscheduled", message);
        }

        [Fact]
        public async Task DeleteJob_SchedulerIsEmptyResult()
        {
            //Act
            var (success, message) = await _scheduler.UnscheduleJobAsync("key");

            //Assert
            Assert.True(success);
            Assert.Equal("Scheduler is empty", message);
        }
    }
}