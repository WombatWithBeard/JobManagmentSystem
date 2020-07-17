using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Scheduler.UnitTests.SchedulerServiceTests
{
    public class DeleteAllJobsUnitTests
    {
        private readonly IScheduler _scheduler;
        private readonly TestJobMaker _jobMaker;

        public DeleteAllJobsUnitTests()
        {
            _jobMaker = new TestJobMaker();
            _scheduler = new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);
        }

        [Fact]
        public async Task DeleteJob_ValidResult()
        {
            //Act
            await _scheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            await _scheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            await _scheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            var (success, message) = await _scheduler.UnscheduleAllJobsAsync();

            //Assert
            Assert.True(success);
            Assert.Equal("All job was successfully unscheduled", message);
        }        
        
        [Fact]
        public async Task DeleteJob_ScheduleIsEmptyResult()
        {
            //Act
            var (success, message) = await _scheduler.UnscheduleAllJobsAsync();

            //Assert
            Assert.True(success);
            Assert.Equal("Scheduler is empty", message);
        }
    }
}