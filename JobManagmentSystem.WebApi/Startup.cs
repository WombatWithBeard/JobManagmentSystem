using JobManagmentSystem.Application;
using JobManagmentSystem.Application.Common.Interfaces;
using JobManagmentSystem.FileStorage;
using JobManagmentSystem.Scheduler;
using JobManagmentSystem.Scheduler.Common.Interfaces;
using JobManagmentSystem.WebApi.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace JobManagmentSystem.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<FileStorage.FileStorage>(Configuration.GetSection(FileStorage.FileStorage.Storage));

            services.AddSingleton<Scheduler.Scheduler>();
            services.AddScoped<IPersistStorage, JobsFileStorage>();
            services.AddScoped<IScheduler, PersistentScheduler>();
            services.AddScoped<TaskFactory>();
            services.AddScoped<IJobService, JobService>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCustomExceptionHandler();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}