using System.Reflection.Metadata;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Scheduler.UnitTests.SchedulerServiceTests
{
    public class AddJobUnitTests
    {
        private readonly IScheduler _scheduler;
        private readonly TestJobMaker _jobMaker;

        public AddJobUnitTests()
        {
            _jobMaker = new TestJobMaker();
            _scheduler = new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>.Instance);
        }

        [Fact]
        public async Task AddJob_ValidResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            var (success, message) = await _scheduler.ScheduleJobAsync(job);

            //Assert
            Assert.True(success);
            Assert.Equal($"Job {job.Key} was successfully scheduled", message);
        }

        [Fact]
        public async Task AddJob_KeyAlreadyExistsResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            await _scheduler.ScheduleJobAsync(job);
            var (success, message) = await _scheduler.ScheduleJobAsync(job);

            //Assert
            Assert.False(success);
            Assert.Equal($"Job {job.Key} already exists", message);
        }
    }
}