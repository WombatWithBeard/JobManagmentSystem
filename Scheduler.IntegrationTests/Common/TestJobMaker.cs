using System;
using System.Globalization;
using ConsoleWriterJobService;
using JobManagmentSystem.Application;
using JobManagmentSystem.Scheduler.Models;
using Microsoft.Extensions.Logging.Abstractions;

namespace Scheduler.IntegrationTests.Common
{
    public class TestJobMaker
    {
        public Job CreateTestJob() => new Job(new ConsoleWriteJobTask(NullLogger.Instance),
            DateTime.Now,
            4,
            0,
            "ConsoleWrite",
            new {Message = "Integration"});

        public Job CreateTestJob(string jobKey) => new Job(new ConsoleWriteJobTask(NullLogger.Instance),
            DateTime.Now,
            4,
            0,
            "ConsoleWrite",
            new
            {
                Message = "Integration"
            }, jobKey);

        public JobDto CreateJobDto() => new JobDto
        {
            Interval = 3,
            Name = "ConsoleWrite",
            IntervalType = 0,
            TimeStart = DateTime.Now.ToString(CultureInfo.CurrentCulture)
        };

        public JobDto CreateJobDto(string toString) => new JobDto
        {
            Key = toString,
            Interval = 3,
            Name = "ConsoleWrite",
            IntervalType = 0,
            TimeStart = DateTime.Now.ToString(CultureInfo.CurrentCulture)
        };
    }
}