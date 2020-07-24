using JobManagmentSystem.FileStorage;
using JobManagmentSystem.Scheduler;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Scheduler.IntegrationTests.Common
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureTestServices(services =>
            {
                // var scheduler =
                //     new JobManagmentSystem.Scheduler.Scheduler(NullLogger<JobManagmentSystem.Scheduler.Scheduler>
                //         .Instance);
                // var options = Options.Create(new FileStorage
                //     {StoragePath = "integrationTests.ndjson"});
                // var storage = new JobsFileStorage(NullLogger<JobsFileStorage>.Instance, options);
                // var persistentScheduler = new PersistentScheduler(scheduler, storage,
                //     NullLogger<PersistentScheduler>.Instance);
                // var testJob = new TestJobMaker();
                //
                // for (int i = 0; i < 10; i++)
                // {
                //     persistentScheduler.ScheduleJobAsync(testJob.CreateTestJob($"test{i}"));
                // }
            });
            base.ConfigureWebHost(builder);
        }
    }
}