using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
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
                CreateHostBuilder(args).Build().Run();
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
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}