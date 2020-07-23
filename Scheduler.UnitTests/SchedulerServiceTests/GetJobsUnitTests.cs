using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Scheduler.UnitTests.SchedulerServiceTests
{
    public class GetJobsUnitTests
    {
        private readonly IScheduler _scheduler;
        private readonly TestJobMaker _jobMaker;

        public GetJobsUnitTests()
        {
            _jobMaker = new TestJobMaker();
            _scheduler =
                new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);
        }

        [Fact]
        public async Task GetJobs_ValidResult()
        {
            //Act
            await _scheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            await _scheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            await _scheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            await _scheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            await _scheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            var result = await _scheduler.GetJobs();

            //Assert
            Assert.True(result.Success);
            Assert.Equal(5, result.Value.Length);
            // Assert.Equal("That's ur jobs, boy", message);
        }

        [Fact]
        public async Task GetJobs_EmptyResult()
        {
            //Act
            var result = await _scheduler.GetJobs();

            //Assert
            Assert.True(result.Failure);
            Assert.Equal("Scheduler is empty", result.Error);
        }
    }
}