using System.IO;
using System.Net;
using JobManagmentSystem.FileStorage;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.Scheduler.Models;
using JobManagmentSystem.WebApi.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;

namespace Scheduler.IntegrationTests.Common
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // var sp = services.BuildServiceProvider();
                //
                // using var scope = sp.CreateScope();
                // var provider = scope.ServiceProvider;
                //
                // var scheduler = provider.GetRequiredService<JobManagmentSystem.Scheduler.Scheduler>();
                // var storage = provider.GetRequiredService<IPersistStorage>();
                // var persistScheduler = new PersistentScheduler(scheduler, storage, NullLogger<PersistentScheduler>.Instance);
                // var testJobMaker = new TestJobMaker();
                //
                // if (File.Exists(Directory.GetCurrentDirectory() + @"\" + "jobs.ndjson"))
                // {
                //     File.Delete(Directory.GetCurrentDirectory() + @"\" + "jobs.ndjson");
                //     // File.Create(Directory.GetCurrentDirectory() + @"\" + "jobs.ndjson");
                // }
                //
                // for (int i = 0; i < 20; i++)
                // {
                //     persistScheduler.ScheduleJobAsync(testJobMaker.CreateTestJob($"test{i}"));
                // }
            });

            base.ConfigureWebHost(builder);
        }
    }
}