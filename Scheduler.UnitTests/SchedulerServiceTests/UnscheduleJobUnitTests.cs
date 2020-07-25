using System.Threading.Tasks;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Scheduler.UnitTests.SchedulerServiceTests
{
    public class UnscheduleJobUnitTests
    {
        private readonly IScheduler _scheduler;
        private readonly TestJobMaker _jobMaker;

        public UnscheduleJobUnitTests()
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
            var result = await _scheduler.UnscheduleJobAsync(job.Key);

            //Assert
            Assert.True(result.Success);
        }

        [Fact]
        public async Task DeleteJob_SchedulerIsEmptyResult()
        {
            //Act
            var result = await _scheduler.UnscheduleJobAsync("key");

            //Assert
            Assert.True(result.Failure);
            Assert.Equal(SchedulerConsts.SchedulerIsEmpty, result.Error);
        }
    }
}