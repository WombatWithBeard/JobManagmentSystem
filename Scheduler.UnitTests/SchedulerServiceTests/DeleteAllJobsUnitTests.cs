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
            _scheduler = new SchedulerService(NullLogger<SchedulerService>.Instance);
        }

        [Fact]
        public void DeleteJob_ValidResult()
        {
            //Act
            _scheduler.AddJob(_jobMaker.CreateTestJob());
            _scheduler.AddJob(_jobMaker.CreateTestJob());
            _scheduler.AddJob(_jobMaker.CreateTestJob());
            var (success, message) = _scheduler.DeleteAllJobs();

            //Assert
            Assert.True(success);
            Assert.Equal("All job was successfully unscheduled", message);
        }        
        
        [Fact]
        public void DeleteJob_ScheduleIsEmptyResult()
        {
            //Act
            var (success, message) = _scheduler.DeleteAllJobs();

            //Assert
            Assert.True(success);
            Assert.Equal("Scheduler is empty", message);
        }
    }
}