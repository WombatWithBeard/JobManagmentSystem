using System.Threading.Tasks;
using JobManagmentSystem.Scheduler.Common.Models;

namespace JobManagmentSystem.Scheduler.Common.Interfaces
{
    public interface IScheduler
    {
        Task<(bool success, string message)> ScheduleJobAsync(Job job);
        Task<(bool success, string message)> RescheduleJobAsync(Job job);
        Task<(bool success, string message)> UnscheduleJobByIdAsync(string key);
        Task<(bool success, string message)> UnscheduleAllJobsAsync();
    }
}