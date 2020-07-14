using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace JobManagmentSystem.Scheduler.Tools
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddScheduler(this IServiceCollection service)
        {
            service.AddSingleton<IScheduler, SchedulerService>();

            return service;
        }
    }
}