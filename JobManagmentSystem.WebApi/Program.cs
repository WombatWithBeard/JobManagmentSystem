using System;
using System.IO;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.WebApi.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace JobManagmentSystem.WebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(new RenderedCompactJsonFormatter(),
                    Directory.GetCurrentDirectory() +
                    $"/logs/{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day}_log.ndjson",
                    LogEventLevel.Information)
                .WriteTo.File(new RenderedCompactJsonFormatter(),
                    Directory.GetCurrentDirectory() +
                    $"/logs/{DateTime.Now.Year}/{DateTime.Now.Month}/{DateTime.Now.Day}_error.ndjson",
                    LogEventLevel.Error)
                .CreateLogger();

            try
            {
                Log.Information("Starting up");
                var host = CreateHostBuilder(args).Build();

                using (var scope = host.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;

                    var scheduler = services.GetRequiredService<Scheduler.Scheduler>();
                    var storage = services.GetRequiredService<IPersistStorage>();
                    RescheduleJobs.Reschedule(scheduler, storage);
                }
                
                host.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}