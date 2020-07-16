#nullable enable
using System;
using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace Scheduler.UnitTests
{
    public class TestJob : IJobTask
    {
        private readonly string _text;

        public TestJob(string text)
        {
            _text = text;
        }

        public Task Invoke(object? state)
        {
            Console.WriteLine(_text);
            return Task.CompletedTask;
        }
    }
}