﻿using JobManagmentSystem.Scheduler;
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
            _scheduler = new SchedulerService(NullLogger<SchedulerService>.Instance);
        }
        
        [Fact]
        public void DeleteJob_ValidResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            _scheduler.AddJob(job);
            var (success, message) = _scheduler.DeleteJobById(job.Key);

            //Assert
            Assert.True(success);
            Assert.Equal($"Job {job.Key} was successfully unscheduled", message);
        }
        
        [Fact]
        public void DeleteJob_JobNotExistsResult()
        {
            //Arrange
            var job = _jobMaker.CreateTestJob();

            //Act
            _scheduler.AddJob(job);
            var (success, message) = _scheduler.DeleteJobById("key");

            //Assert
            Assert.True(success);
            Assert.Equal("Job does not exist", message);
        }
        
        [Fact]
        public void DeleteJob_SchedulerIsEmptyResult()
        {
            //Act
            var (success, message) = _scheduler.DeleteJobById("key");

            //Assert
            Assert.True(success);
            Assert.Equal("Scheduler is empty", message);
        }
    }
}