using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Scheduler.UnitTests.SchedulerServiceTests
{
    public class GetJobsArrayUnitTests
    {
        private readonly IScheduler _scheduler;
        private readonly TestJobMaker _jobMaker;

        public GetJobsArrayUnitTests()
        {
            _jobMaker = new TestJobMaker();
            _scheduler =
                new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);
        }

        [Fact]
        public async Task GetJobsArray_ValidResult()
        {
            //Act
            await _scheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            await _scheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            await _scheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            await _scheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            await _scheduler.ScheduleJobAsync(_jobMaker.CreateTestJob());
            var (success, message, jobs) = await _scheduler.GetJobs();

            //Assert
            Assert.True(success);
            Assert.Equal(5, jobs.Length);
            Assert.Equal("That's ur jobs, boy", message);
        }        
        
        [Fact]
        public async Task GetJobsArray_EmptyResult()
        {
            //Act
            var (success, message, jobs) = await _scheduler.GetJobs();

            //Assert
            Assert.False(success);
            Assert.Null(jobs);
            Assert.Equal("Scheduler is empty", message);
        }
    }
}