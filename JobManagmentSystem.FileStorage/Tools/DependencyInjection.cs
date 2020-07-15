using JobManagmentSystem.Scheduler.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace JobManagmentSystem.FileStorage.Tools
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddJobsFileStorage(this IServiceCollection service)
        {
            service.AddSingleton<IPersistStorage, JobsFileStorage>();

            return service;
        }
    }
}