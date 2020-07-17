using JobManagmentSystem.Scheduler;
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
        public async void DeleteJob_ValidResult()
        {
            //Act
            await _scheduler.ScheduleJob(_jobMaker.CreateTestJob());
            await _scheduler.ScheduleJob(_jobMaker.CreateTestJob());
            await _scheduler.ScheduleJob(_jobMaker.CreateTestJob());
            var (success, message) = await _scheduler.UnscheduleAllJobs();

            //Assert
            Assert.True(success);
            Assert.Equal("All job was successfully unscheduled", message);
        }        
        
        [Fact]
        public void DeleteJob_ScheduleIsEmptyResult()
        {
            //Act
            var (success, message) = _scheduler.UnscheduleAllJobs();

            //Assert
            Assert.True(success);
            Assert.Equal("Scheduler is empty", message);
        }
    }
}