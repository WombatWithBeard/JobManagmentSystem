using System;
using JobManagmentSystem.Scheduler.Common.Models;

namespace Scheduler.UnitTests
{
    public class TestJobMaker
    {
        public Job CreateTestJob()
        {
            return new Job(new TestJob("Valid result"),
                DateTime.Now,
                4,
                10,
                "Console",
                new object());
        }
    }
}