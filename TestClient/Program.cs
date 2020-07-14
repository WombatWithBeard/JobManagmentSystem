#nullable enable
using System;
using System.Threading.Tasks;
using FileCopyJobService;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Interfaces;

namespace TestClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // IScheduler s = SchedulerService.Instance;
            // var task = new FileCopyJob("хуй", "говно");
            var sch = new Schedule(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(10));
            var timer = new Scheduler();

            // s.AddJob(Guid.NewGuid().ToString(), timer.Schedule(task, sch));

            await Task.Delay(TimeSpan.FromMinutes(10));

            Console.ReadLine();
        }
    }
}